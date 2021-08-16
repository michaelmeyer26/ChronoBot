using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ChronoBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace ChronoBot
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly ServiceProvider _services;

        public Program(CommandService commands = null, DiscordSocketClient client = null)
        {
            _client = client ?? new DiscordSocketClient();
            _commands = commands ?? new CommandService();
            _services = (ServiceProvider)BuildServiceProvider();
            

            _client.Log += Log;
            _commands.Log += Log;
        }

        public async Task MainAsync()
        {
            var token = File.ReadAllText("token.txt");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            CommandHandler handler = new CommandHandler(_services, _commands, _client);
            await handler.InstallCommandsAsync();
            

            // Block this task until program is closed.
            await Task.Delay(-1);
        }

        public IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton<HttpService>()
            .BuildServiceProvider();

        // Logs message to console
        // Replace with logging messages to a text file eventually
        // Figure out a better long-term solution after that
        private Task Log(LogMessage msg)
        {
            Console.Write(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
