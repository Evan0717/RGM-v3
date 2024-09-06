using Discord;
using Discord.WebSocket;
using Mono.Cecil;
using System;
using System.Threading.Tasks;

public class BotService
{
    private readonly DiscordSocketClient _client;

    public BotService()
    {
        _client = new DiscordSocketClient();
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
    }

    public async Task InitializeAsync(string token)
    {
        await _client.LoginAsync(Discord.TokenType.Bot, token);
        await _client.StartAsync();
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private Task ReadyAsync()
    {
        Console.WriteLine("Bot is connected!");
        return Task.CompletedTask;
    }
}
