using DataLayer;
using LogicLayer;
using LogicLayer.Managers;
using LogicLayer.IRepos;
using Microsoft.AspNetCore.Authentication.Cookies;
using DataLayer.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/login");
    });

builder.Services.AddSingleton<IConnection, MySQLConnection>(sp =>
{
    var connection = new MySQLConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
    connection.Open();
    return connection;
});

IServiceCollection serviceCollection = builder.Services.AddSingleton<IUserRepo, UserRepo>();
builder.Services.AddSingleton<IPostRepo, PostRepo>();
builder.Services.AddSingleton<ICommentRepo, CommentRepo>();
builder.Services.AddSingleton<ITournamentRepo, TournamentRepo>();
builder.Services.AddSingleton<IMatchRepo, MatchRepo>();
builder.Services.AddSingleton<IPlayerRepo, PlayerRepo> ();

builder.Services.AddSingleton<UserManager>();
builder.Services.AddSingleton<PostManager>();
builder.Services.AddSingleton<CommentManager>();
builder.Services.AddSingleton<TournamentManager>();
builder.Services.AddSingleton<MatchManager>();
builder.Services.AddSingleton<PlayerManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{ 
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
