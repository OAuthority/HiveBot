using Microsoft.Extensions.DependencyInjection;

namespace HiveBot.Console;

public interface IHiveBot
{
    /// <summary>
    /// Start the bot
    /// </summary>
    /// <returns></returns>
    Task StartAsync( ServiceProvider services );

    /// <summary>
    /// Stop the bot
    /// </summary>
    /// <returns></returns>
    Task StopAsync();
}