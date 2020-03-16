using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Domain;
using ChatServer.Domain.Interfaces;
using ChatServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? Configuration.GetConnectionString("Local");

            Console.WriteLine($"Using connection string: \"{connectionString}\"");

            switch (Configuration.GetValue<string>("Database").ToUpper())
            {
                case "POSTGRESQL":
                    services.AddSingleton<IAuthRepository>(new Repositories.PostgreSQL.AuthRepository(connectionString));
                    services.AddSingleton<IChatRepository>(new Repositories.PostgreSQL.ChatRepository(connectionString));
                    services.AddSingleton<IUserRepository>(new Repositories.PostgreSQL.UserRepository(connectionString));
                    break;
                default:
                    throw new Exception($"Unrecognized database \"{Configuration.GetValue<string>("Database")}\"");
            }

            services.AddScoped(typeof(IUserDomain), typeof(UserDomain));
            services.AddScoped(typeof(IChatDomain), typeof(ChatDomain));
            services.AddScoped(typeof(IAuthDomain), typeof(AuthDomain));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
