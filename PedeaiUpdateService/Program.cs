using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PedeaiUpdateService
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                // Registra como Windows Service (no Linux ignora e roda normal)
                .UseWindowsService(o => o.ServiceName = "PedeaiUpdateService")
                .ConfigureServices(services =>
                {
                    services.AddHostedService<UpdateWorker>();
                })
                .Build()
                .Run();
        }
    }
}
