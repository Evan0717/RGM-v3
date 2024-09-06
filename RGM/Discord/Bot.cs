using Discord;
using Discord.WebSocket;
using Mono.Cecil;
using System;
using System.Threading.Tasks;
using Exiled.API.Features;

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
        Log.Info(log.ToString());
        return Task.CompletedTask;
    }

    private Task ReadyAsync()
    {
        Log.Info("Bot is connected!");
        return Task.CompletedTask;
    }
}
