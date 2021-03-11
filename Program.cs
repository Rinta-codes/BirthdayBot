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
            BirthdayBot bbot = new BirthdayBot();

            //This is where we get the Token value from the configuration file
            await bbot._client.LoginAsync(TokenType.Bot, bbot._config["Token"]);
            await bbot._client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        public BirthdayBot()
        {
            _client = new DiscordSocketClient();

            //Hook into log event and write it out to the console
            _client.Log += LogAsync;

            //Hook into the client ready event
            _client.Ready += ReadyAsync;

            //Hook into the message received event, this is how we handle the hello world example
            _client.MessageReceived += MessageReceivedAsync;

            //Create the configuration
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

        //I wonder if there's a better way to handle commands (spoiler: there is :))
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            //This ensures we don't loop things by responding to ourselves (as the bot)
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "beep")
            {
                await message.Channel.SendMessageAsync("boop");
            }
        }
    }
}