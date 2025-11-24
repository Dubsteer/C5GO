using DataLayer;
using DataLayer.Repos;
using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Managers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace DesktopApp
{
    internal static class Program
    {
        public static ServiceProvider Services;

        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();

            // === DATABASE CONNECTION ===
            services.AddSingleton<IConnection>(sp =>
            {
                var conn = new MySQLConnection("server=127.0.0.1;port=3306;user id=root;password=1234;database=local_dtb;SslMode=none;");
                conn.Open();
                return conn;
            });

            // === REPOS ===
            services.AddTransient<IUserRepo, UserRepo>();
            services.AddTransient<IPostRepo, PostRepo>();
            services.AddTransient<ICommentRepo, CommentRepo>();
            services.AddTransient<ITournamentRepo, TournamentRepo>();
            services.AddTransient<IMatchRepo, MatchRepo>();
            services.AddTransient<IPlayerRepo, PlayerRepo>();

            // === MANAGERS ===
            services.AddTransient<UserManager>();
            services.AddTransient<PostManager>();
            services.AddTransient<CommentManager>();
            services.AddTransient<TournamentManager>();
            services.AddTransient<MatchManager>();
            services.AddTransient<PlayerManager>();

            // === FORMS ===
            services.AddTransient<Login>();
            services.AddTransient<AdminPanel>();

            Services = services.BuildServiceProvider();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(Services.GetRequiredService<Login>());
        }
    }
}
