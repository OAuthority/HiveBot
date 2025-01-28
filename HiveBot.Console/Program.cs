using Microsoft.Extensions.DependencyInjection;

namespace HiveBot.Console;

internal class Program
{
    private static void Main(string[] args) =>
        MainAsync(args).GetAwaiter().GetResult();

    /// <summary>
    /// Main entry point, handle starting and stopping the bot
    /// </summary>
    /// <param name="args"></param>
    private static async Task MainAsync(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped<IHiveBot, HiveBot>()
            .BuildServiceProvider();
        
        try
        {
            IHiveBot hiveBot = serviceProvider.GetRequiredService<IHiveBot>();
            
            await hiveBot.StartAsync(serviceProvider);

            System.Console.WriteLine("Connected to Discord. Press Q key to exit...");
            
            do
            {
                var keyInfo = System.Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.Q)
                {
                    System.Console.WriteLine("\nShutting down!");

                    await hiveBot.StopAsync();
                    return;
                }
            } while (true);
        }
        catch (Exception e)
        {
            // if we encountered an exception, just bail out, please
            // this could be because we didn't find the ini file, or because we didn't have
            // the correct data in the ini file to start the bot
            System.Console.WriteLine(e.Message);
            Environment.Exit(-1);
        }
    }
}