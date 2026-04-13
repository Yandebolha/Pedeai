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

            // Resolve credenciais Supabase
            string supabaseUrl = Configuration["Supabase:Url"]
                                 ?? throw new InvalidOperationException("Supabase:Url n\u00e3o configurado no appsettings.json");
            string supabaseKey = Configuration["Supabase:Key"]
                                 ?? throw new InvalidOperationException("Supabase:Key n\u00e3o configurado no appsettings.json");
            string packagesDir = Configuration["PackagesDir"] ?? "Data/packages";

            services.AddSingleton(new UpdateDb(supabaseUrl, supabaseKey));
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
