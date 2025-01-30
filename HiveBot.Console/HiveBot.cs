using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HiveBot.Core;
using Microsoft.Extensions.DependencyInjection;

namespace HiveBot.Console;

public class HiveBot : IHiveBot
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private ServiceProvider? _serviceProvider;
    
    public HiveBot()
    {
        DiscordSocketConfig config = new()
        {
            // Whilst Discord has deprecated the message content API, it makes sense to use it until
            // it is removed for things like setting long strings of text (see reminder command) until
            // the UI is implemented so that this can be updated on the frontend without using Discord
            // messages itself.
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        
        _client = new DiscordSocketClient(config);
        _commands = new CommandService();
    }

    public async Task StartAsync( ServiceProvider services )
    {
        string discordToken = Configuration.GetConfigVariable("Discord", "Token") ?? 
                              throw new Exception( "Discord API token missing. Please provide it.");
        
        _serviceProvider = services;
        await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
        
        // Start and connect the actual bot
        await _client.LoginAsync(TokenType.Bot, discordToken);
        await _client.StartAsync();

        _client.MessageReceived += HandleCommandAsync;
    }
    
    /// <summary>
    /// Actually stop the bot and disconnect from Discord
    /// </summary>
    public async Task StopAsync()
    {
        if (_client != null)
        {
            await _client.LogoutAsync();
            await _client.StopAsync();
        }
    }

    private async Task HandleCommandAsync( SocketMessage arg )
    {
        if (arg is not SocketUserMessage message || message.Author.IsBot)
        {
            return;
        }
        
        // check if the message begins with a ^, if so, we should action this
        // it is unlikely that a message would begin with a ^ if it isn't meant to be issued
        // to a bot. ! is the standard but other bots could be listening for that
        int position = 0;
        bool messageIsCommand = message.HasCharPrefix('^', ref position);
        
        if (messageIsCommand)
        {
            // Execute the command if it exists in the ServiceCollection, otherwise we can't do 
            // much with it
            await _commands.ExecuteAsync(
                new SocketCommandContext(_client, message),
                position,
                _serviceProvider);

            return;
        }
    }
}