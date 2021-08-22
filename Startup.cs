using System;
using ChatServer.Services;
using ChatServer.Services.Interfaces;
using ChatServer.Exceptions;
using ChatServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ChatServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? Configuration.GetConnectionString("Local");

            services.AddSingleton<IConnectionStringProvider>(new Repositories.PostgreSQL.ConnectionStringProvider(connectionString));

            switch (Configuration.GetValue<string>("Database").ToUpper())
            {
                case "POSTGRESQL":
                    services.AddScoped<IChatRepository, Repositories.PostgreSQL.ChatRepository>();
                    services.AddScoped<IUserRepository, Repositories.PostgreSQL.UserRepository>();
                    services.AddScoped<IFriendRepository, Repositories.PostgreSQL.FriendRepository>();
                    services.AddScoped<IBlockRepository, Repositories.PostgreSQL.BlockRepository>();
                    break;
                default:
                    throw new Exception($"Unrecognized database \"{Configuration.GetValue<string>("Database")}\"");
            }

            services.AddSingleton<ISecretProvider, SecretProvider>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddScoped<IBlockService, BlockService>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
