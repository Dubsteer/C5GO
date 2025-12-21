using DataLayer;
using DataLayer.Repos;
using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Managers;
using LogicLayer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// RAZOR PAGES
// =====================================================
builder.Services.AddRazorPages();

// =====================================================
// AUTHENTICATION
// =====================================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");

// =====================================================
// DATABASE CONNECTION
// =====================================================
builder.Services.AddSingleton<IConnection, MySQLConnection>(sp =>
{
    var conn = new MySQLConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
    conn.Open();
    return conn;
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

// =====================================================
// MANAGERS
// =====================================================
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<PostManager>();
builder.Services.AddScoped<CommentManager>();
builder.Services.AddScoped<MatchManager>();
builder.Services.AddScoped<PlayerManager>();
builder.Services.AddScoped<TeamManager>();
builder.Services.AddScoped<TeamMatchManager>();
builder.Services.AddScoped<TournamentManager>();

// =====================================================
// SERVICES
// =====================================================
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// =====================================================
// MIDDLEWARE
// =====================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
