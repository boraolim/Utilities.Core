using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Utilities;
using TestUtilitiesLibrary;

namespace TestUtilities
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

            /* Lectura de opciones del archivo de configuración. */
            services.Configure<ConnectionStringCollection>(options => config.GetSection($"CollectionConnectionStrings").Bind(options));

            /* Inyectamos la clase 'App' */
            services.AddSingleton<App>();

            /* Datos dummie. */
            services.AddTransient<IDummieData, DummieData>();

            /* Definición de la inyección de servicios de 'Utilities'. */
            services.AddTransient<IToolService, ToolService>();
            services.AddTransient<IDictionaryCollectionService, DictionaryCollectionService>();
            services.AddTransient<IRijndaelEncryptionService, RijndaelEncryptionService>();
            services.AddTransient<ISMSService, SMSService>();
            services.AddTransient<IGoogleRepositoryService, GoogleRepositoryService>();
            services.AddTransient(typeof(IXMLSerializationService<>), typeof(XMLSerializationService<>));
            services.AddTransient(typeof(IDbManagerService), typeof(DbManagerService));

            /* Otros servicios de la aplicación de la consola. */
            services.AddTransient<IInterfaceSample, Classsample>();
            //services.AddTransient<IUser, User>();

            return services;
        }

    }
}
