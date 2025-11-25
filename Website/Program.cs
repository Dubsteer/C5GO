using DataLayer;
using DataLayer.Repos;
using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Managers;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// ==========================================
// AUTHENTICATION
// ==========================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
    });

// ==========================================
// DATABASE CONNECTION  (SINGLETON — FIXED)
// ==========================================
builder.Services.AddSingleton<IConnection, MySQLConnection>(sp =>
{
    var conn = new MySQLConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );

    conn.Open();  // IMPORTANT: Keep connection open (your old system depends on this)
    return conn;
});

// ==========================================
// REPOSITORIES (SCOPED)
// ==========================================
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddScoped<ICommentRepo, CommentRepo>();
builder.Services.AddScoped<ITournamentRepo, TournamentRepo>();
builder.Services.AddScoped<IMatchRepo, MatchRepo>();
builder.Services.AddScoped<IPlayerRepo, PlayerRepo>();

// TEAMS (NEW)
builder.Services.AddScoped<ITeamRepo, TeamRepo>();

// ==========================================
// MANAGERS (SCOPED)
// ==========================================
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<PostManager>();
builder.Services.AddScoped<CommentManager>();
builder.Services.AddScoped<TournamentManager>();
builder.Services.AddScoped<MatchManager>();
builder.Services.AddScoped<PlayerManager>();

// TEAMS (NEW)
builder.Services.AddScoped<TeamManager>();


var app = builder.Build();

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
