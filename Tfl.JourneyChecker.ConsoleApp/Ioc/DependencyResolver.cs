using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Tfl.JourneyChecker.ConsoleApp.Enums;
using Tfl.JourneyChecker.ConsoleApp.Models;
using Tfl.JourneyChecker.ConsoleApp.Wrappers;

namespace Tfl.JourneyChecker.ConsoleApp.Ioc
{
    public class DependencyResolver
    {
        public static ContainerBuilder RegisterDependencies()
        {
            var serviceCollection = new ServiceCollection();
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            var configuration = configurationBuilder.Build();

            // Adding Configuration.
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            var loggingFilePath = configuration.GetValue<string>("Logging:Path");

            // Adding logging support
            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(loggingFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            serviceCollection.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Error);
                builder.AddSerilog(logger: serilogLogger, dispose: true);
            });

            serviceCollection.Configure<TflSettings>(configuration.GetSection("Tfl:Api"));

            // MediatR add on will register handlers in this program assembly.
            // you dont need to register any dependencies.  
            serviceCollection.AddMediatR(typeof(Program).Assembly); 
            var builder = new ContainerBuilder();
            builder.Populate(serviceCollection); 

            // Registering customer dependencies
            builder.Register(
               context =>
                   context.IsRegistered<IHttpClientFactory>()
                       ? context.Resolve<IHttpClientFactory>().CreateClient()
                       : new HttpClient()).As<HttpClient>();
            builder.RegisterType<ConsoleWrapper>().As<IConsoleWrapper>().InstancePerLifetimeScope();

            
            var journeyTypeString = configuration.GetValue<string>("JourneyType");
            if(!Enum.TryParse(journeyTypeString, out JourneyType journeyType))
            {
                throw new Exception("The JourneyType appsetting is invalid. It must to be Road.");
            }

            // Notes: Please register new jouney type check if you it needs to be. 
            switch (journeyType)
            {
                case JourneyType.Road:
                    builder.RegisterType<RoadJourneyChecker>().As<IJourneyChecker>();
                    break;
                default:
                    builder.RegisterType<RoadJourneyChecker>().As<IJourneyChecker>();
                    break;

            }
            return builder;
        }
    }
}
