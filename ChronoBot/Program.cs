using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace ChronoBot
{
    public class Program
    {

        private DiscordSocketClient _client;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            var token = File.ReadAllText("token.txt");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until program is closed.
            await Task.Delay(-1);
        }

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
