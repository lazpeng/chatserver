using System;
using ChatServer.Domain;
using ChatServer.Domain.Interfaces;
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

            Console.WriteLine($"Using connection string: \"{connectionString}\"");

            switch (Configuration.GetValue<string>("Database").ToUpper())
            {
                case "POSTGRESQL":
                    services.AddScoped<IAuthRepository, Repositories.PostgreSQL.AuthRepository>();
                    services.AddScoped<IChatRepository, Repositories.PostgreSQL.ChatRepository>();
                    services.AddScoped<IUserRepository, Repositories.PostgreSQL.UserRepository>();
                    break;
                default:
                    throw new Exception($"Unrecognized database \"{Configuration.GetValue<string>("Database")}\"");
            }

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IUserService, UserService>();

            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(x => x.FullName);
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatServer API", Version = "v1" });
            });

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

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatServer API");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
