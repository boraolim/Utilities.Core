using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TestUtilities.DBSample.Main;
using TestUtilities.DBSample.ContextDb;

namespace TestUtilities.DBSample
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.GetService<App>().RunAsync(args);
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile($"AppSettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);

            /* Contextos de Bases de Datos. */
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            );

            /* Inyectamos la clase 'App' */
            services.AddSingleton<App>();
            /* Otros servicios de la aplicación de la consola. */
            //services.AddTransient<IUser, User>();

            return services;
        }

    }

    public class DetailDTO
    {
        public int IdDetail { get; set; }
        public Guid IdHeader { get; set; }
        public string DetailAttribute1 { get; set; }
    }

    public class HeaderDTO
    {
        public Guid IdHeader { get; set; }
        public string HeaderAttribute1 { get; set; }
        public DateTime? TransactionDate { get; set; }
        public IEnumerable<DetailDTO> DetailTable { get; set; }
    }
}
