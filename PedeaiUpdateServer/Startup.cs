using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PedeaiUpdateServer.Data;
using System;

namespace PedeaiUpdateServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration) { Configuration = configuration; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Resolve caminhos
            string connStr     = Configuration.GetConnectionString("Supabase")
                                 ?? throw new InvalidOperationException("ConnectionStrings:Supabase não configurado no appsettings.json");
            string packagesDir = Configuration["PackagesDir"] ?? "Data/packages";

            services.AddSingleton(new UpdateDb(connStr));
            services.AddSingleton(packagesDir);

            // Permite uploads grandes (pacotes de atualização)
            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
            {
                o.MultipartBodyLengthLimit = 500_000_000; // 500 MB
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
