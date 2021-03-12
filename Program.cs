using System;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace BirthdayBot
{
    class BirthdayBot
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;

        static async Task Main(string[] args) // I heard new C# supports async Main method... 
        {
            // Start up the bot process
            BirthdayBot bbot = new BirthdayBot();

            // Get the Token value from the configuration file and use it to login (but not go online)
            await bbot._client.LoginAsync(TokenType.Bot, bbot._config["Token"]);

            // Start up the connection (go online)
            // Will immediately return after being called, initialising the connection on another thread
            await bbot._client.StartAsync();

            // Block the program until it is closed, so that Bot keeps running after connecting (seems rather crude)
            await Task.Delay(-1);
        }

        public BirthdayBot()
        {
            _client = new DiscordSocketClient();

            // Hook into log event and write it out to the console
            _client.Log += LogAsync;

            // Hook into the client ready event
            _client.Ready += ReadyAsync;

            // Hook into the message received event, this is how we handle the hello world example
            _client.MessageReceived += MessageReceivedAsync;

            // Load configuration from config file into a variable
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
            _config = _builder.Build();
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected...");
            return Task.CompletedTask;
        }

        // Directly hook into Messages event
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            //This ensures we don't loop things by responding to ourselves
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "beep")
            {
                await message.Channel.SendMessageAsync("boop");
            }
        }
    }
}