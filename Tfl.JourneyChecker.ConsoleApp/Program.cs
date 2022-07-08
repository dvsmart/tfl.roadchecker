
using Autofac;
using Tfl.JourneyChecker.ConsoleApp;
using Tfl.JourneyChecker.ConsoleApp.Enums;
using Tfl.JourneyChecker.ConsoleApp.Ioc;
using Tfl.JourneyChecker.ConsoleApp.Models;

public class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("Started TFL Journey status checker");
        
        // Register dependencies. 
        var container = DependencyResolver.RegisterDependencies().Build(); 
        var roadJourneyChecker = container.Resolve<IJourneyChecker>();

        if (args == null || args.Length < 1)
        {
            Console.WriteLine("You must pass road id as an argument.");
            Console.WriteLine("Existing TFL Journey status checker");
            return (int)(int)ResultStatus.ValidationError; 
        }

        // Process Request
        return await roadJourneyChecker.CheckStatus(args[0]); 
    }
}

