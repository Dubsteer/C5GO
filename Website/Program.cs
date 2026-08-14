using DataLayer;
using DataLayer.Repos;
using LogicLayer.IRepos;
using LogicLayer.Managers;
using LogicLayer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.FileProviders;
using System.Threading.RateLimiting;
using System.Security.Claims;
using Website.Configuration;
using Website.Services;

var builder = WebApplication.CreateBuilder(args);

var turnstileConfiguration = builder.Configuration
    .GetSection(TurnstileOptions.SectionName)
    .Get<TurnstileOptions>() ?? new TurnstileOptions();
if (builder.Environment.IsDevelopment() && !turnstileConfiguration.IsConfigured)
{
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Turnstile:SiteKey"] = TurnstileOptions.DevelopmentSiteKey,
        ["Turnstile:SecretKey"] = TurnstileOptions.DevelopmentSecretKey
    });
    turnstileConfiguration = builder.Configuration
        .GetSection(TurnstileOptions.SectionName)
        .Get<TurnstileOptions>() ?? new TurnstileOptions();
}

builder.Services.Configure<FeatureOptions>(
    builder.Configuration.GetSection(FeatureOptions.SectionName));
builder.Services.Configure<TurnstileOptions>(
    builder.Configuration.GetSection(TurnstileOptions.SectionName));

if (builder.Environment.IsProduction() && !turnstileConfiguration.IsConfigured)
{
    throw new InvalidOperationException(
        "Turnstile configuration is missing. Configure Turnstile:SiteKey and Turnstile:SecretKey.");
}

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
        options.Events.OnValidatePrincipal = async context =>
        {
            var featureOptions = context.HttpContext.RequestServices
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<FeatureOptions>>()
                .Value;
            if (!featureOptions.CommunityEnabled ||
                !int.TryParse(context.Principal?.FindFirst("id")?.Value, out var userId))
            {
                return;
            }

            var roleManager = context.HttpContext.RequestServices
                .GetRequiredService<RoleManager>();
            HashSet<string> expectedRoles;
            try
            {
                expectedRoles = roleManager.GetRolesForUser(userId)
                    .Select(role => role.ToString())
                    .ToHashSet(StringComparer.Ordinal);
            }
            catch (InvalidOperationException)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }
            var currentRoles = context.Principal!
                .FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToHashSet(StringComparer.Ordinal);

            if (expectedRoles.SetEquals(currentRoles))
                return;

            var identity = (ClaimsIdentity)context.Principal.Identity!;
            foreach (var claim in identity.FindAll(ClaimTypes.Role).ToArray())
                identity.RemoveClaim(claim);
            foreach (var role in expectedRoles)
                identity.AddClaim(new Claim(ClaimTypes.Role, role));

            context.ReplacePrincipal(new ClaimsPrincipal(identity));
            context.ShouldRenew = true;
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole(
                  LogicLayer.Enums.PlatformRole.Owner.ToString(),
                  LogicLayer.Enums.PlatformRole.Admin.ToString()));

    options.AddPolicy("ModeratorOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole(
                  LogicLayer.Enums.PlatformRole.Owner.ToString(),
                  LogicLayer.Enums.PlatformRole.Admin.ToString(),
                  LogicLayer.Enums.PlatformRole.Moderator.ToString()));
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
    options.AddPolicy("community", context => CreateFixedWindowLimiter(context, 120, TimeSpan.FromMinutes(1)));
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
builder.Services.AddScoped<IRoleRepo, RoleRepo>();
builder.Services.AddScoped<ICommunityRepo, CommunityRepo>();

builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<PostManager>();
builder.Services.AddScoped<CommentManager>();
builder.Services.AddScoped<TournamentManager>();
builder.Services.AddScoped<MatchManager>();
builder.Services.AddScoped<PlayerManager>();
builder.Services.AddScoped<TeamManager>();
builder.Services.AddScoped<TeamMatchManager>();
builder.Services.AddScoped<NotificationManager>();
builder.Services.AddScoped<RoleManager>();
builder.Services.AddScoped<CommunityManager>();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<PostImageStorage>();
builder.Services.AddScoped<CommunityImageStorage>();
builder.Services.AddScoped<UserRoleClaimsService>();
builder.Services.AddHttpClient<TurnstileService>(client =>
{
    client.BaseAddress = new Uri("https://challenges.cloudflare.com/turnstile/v0/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

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
        "frame-src https://www.youtube-nocookie.com https://challenges.cloudflare.com; " +
        "img-src 'self' data: https:; " +
        "object-src 'none'; " +
        "script-src 'self' https://challenges.cloudflare.com; " +
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

var communityImagesDirectory = Path.Combine(
    app.Environment.WebRootPath,
    "Images",
    "community");
Directory.CreateDirectory(communityImagesDirectory);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(communityImagesDirectory),
    RequestPath = "/images/community"
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

public partial class Program
{
}
