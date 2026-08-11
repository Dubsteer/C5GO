using DataLayer;
using DataLayer.Repos;
using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Managers;
using LogicLayer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.FileProviders;
using System.Threading.RateLimiting;
using Website.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://localhost:5063");
}

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 1;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Errors/403";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Admin"));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
});

var dataProtection = builder.Services
    .AddDataProtection()
    .SetApplicationName("C5GO");

var dataProtectionKeysPath = builder.Configuration["DataProtection:KeysPath"];
if (!string.IsNullOrWhiteSpace(dataProtectionKeysPath))
{
    var fullKeysPath = Path.IsPathRooted(dataProtectionKeysPath)
        ? dataProtectionKeysPath
        : Path.Combine(builder.Environment.ContentRootPath, dataProtectionKeysPath);

    Directory.CreateDirectory(fullKeysPath);
    dataProtection.PersistKeysToFileSystem(new DirectoryInfo(fullKeysPath));
}

builder.Services.AddSingleton<PasswordResetTokenService>();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", context => CreateFixedWindowLimiter(context, 10, TimeSpan.FromMinutes(5)));
    options.AddPolicy("register", context => CreateFixedWindowLimiter(context, 5, TimeSpan.FromHours(1)));
    options.AddPolicy("password-reset", context => CreateFixedWindowLimiter(context, 5, TimeSpan.FromMinutes(15)));
});

builder.Services.AddScoped<IConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connStr = configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connStr))
        throw new InvalidOperationException(
            "Connection string 'DefaultConnection' is missing. Configure it with User Secrets or an environment variable.");

    return new MySQLConnection(connStr);
});

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddScoped<ICommentRepo, CommentRepo>();
builder.Services.AddScoped<ITournamentRepo, TournamentRepo>();
builder.Services.AddScoped<IMatchRepo, MatchRepo>();
builder.Services.AddScoped<IPlayerRepo, PlayerRepo>();
builder.Services.AddScoped<ITeamRepo, TeamRepo>();
builder.Services.AddScoped<ITeamMatchRepo, TeamMatchRepo>();
builder.Services.AddScoped<INotificationRepo, NotificationRepo>();

builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<PostManager>();
builder.Services.AddScoped<CommentManager>();
builder.Services.AddScoped<TournamentManager>();
builder.Services.AddScoped<MatchManager>();
builder.Services.AddScoped<PlayerManager>();
builder.Services.AddScoped<TeamManager>();
builder.Services.AddScoped<TeamMatchManager>();
builder.Services.AddScoped<NotificationManager>();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<PostImageStorage>();

var pandaScoreApiKey = builder.Configuration["PandaScore:ApiKey"];

if (string.IsNullOrWhiteSpace(pandaScoreApiKey))
{
    builder.Services.AddScoped<IExternalMatchProvider, MockExternalMatchProvider>();
}
else
{
    builder.Services.AddHttpClient<IExternalMatchProvider, PandaScoreMatchProvider>(client =>
    {
        client.BaseAddress = new Uri("https://api.pandascore.co");
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pandaScoreApiKey);
    });
}

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "base-uri 'self'; " +
        "connect-src 'self'; " +
        "font-src 'self' data:; " +
        "form-action 'self'; " +
        "frame-ancestors 'none'; " +
        "frame-src https://www.youtube-nocookie.com; " +
        "img-src 'self' data: https:; " +
        "object-src 'none'; " +
        "script-src 'self'; " +
        "style-src 'self';";

    await next();
});

var postImagesDirectory = Path.Combine(
    app.Environment.WebRootPath,
    "Images",
    "posts");
Directory.CreateDirectory(postImagesDirectory);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(postImagesDirectory),
    RequestPath = "/images/posts"
});
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

static RateLimitPartition<string> CreateFixedWindowLimiter(
    HttpContext context,
    int permitLimit,
    TimeSpan window)
{
    var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    return RateLimitPartition.GetFixedWindowLimiter(
        partitionKey,
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = window,
            QueueLimit = 0
        });
}
