using DataLayer;
using DataLayer.Repos;
using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Managers;
using LogicLayer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CONFIGURATION
// =====================================================
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// =====================================================
// FORCE HTTP (LOCAL / NGROK)
// =====================================================
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://localhost:5063");
}

// =====================================================
// AUTHENTICATION (COOKIE)
// =====================================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
    });

// =====================================================
// AUTHORIZATION (ADMIN POLICY)
// =====================================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.Identity!.Name == "admin"
              ));
});

// =====================================================
// RAZOR PAGES + ADMIN FOLDER LOCK
// =====================================================
builder.Services.AddRazorPages(options =>
{
    // ?? CIJELI /Admin folder je zaklju?an za ADMIN-a
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
});

// =====================================================
// DATABASE CONNECTION
// =====================================================
builder.Services.AddScoped<IConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connStr = configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connStr))
        throw new Exception("Connection string 'DefaultConnection' is missing.");

    return new MySQLConnection(connStr);
});

// =====================================================
// REPOSITORIES
// =====================================================
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddScoped<ICommentRepo, CommentRepo>();
builder.Services.AddScoped<ITournamentRepo, TournamentRepo>();
builder.Services.AddScoped<IMatchRepo, MatchRepo>();
builder.Services.AddScoped<IPlayerRepo, PlayerRepo>();
builder.Services.AddScoped<ITeamRepo, TeamRepo>();
builder.Services.AddScoped<ITeamMatchRepo, TeamMatchRepo>();
builder.Services.AddScoped<INotificationRepo, NotificationRepo>();

// =====================================================
// MANAGERS
// =====================================================
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<PostManager>();
builder.Services.AddScoped<CommentManager>();
builder.Services.AddScoped<TournamentManager>();
builder.Services.AddScoped<MatchManager>();
builder.Services.AddScoped<PlayerManager>();
builder.Services.AddScoped<TeamManager>();
builder.Services.AddScoped<TeamMatchManager>();
builder.Services.AddScoped<NotificationManager>();

// =====================================================
// SERVICES
// =====================================================
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// =====================================================
// MIDDLEWARE PIPELINE
// =====================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // ? isklju?eno zbog local/ngrok

app.UseStaticFiles();
app.UseRouting();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
