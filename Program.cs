using System;
using System.Collections.Generic;
using System.Linq;
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

        static async Task Main(string[] args) // I heard modern C# supports async Main method... 
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
            // This ensures we don't loop things by responding to ourselves
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            // This is a basic hardcoded response to a hardcoded message
            if (message.Content == "beep")
            {
                await message.Channel.SendMessageAsync("boop");
            }

            // This command assigns role "Birthday Cake" to user who executed it
            if (message.Content == "poke")
            {
                SocketGuildUser author = message.Author as SocketGuildUser;
                if (author != null) // If message was not from a user in a server -- author will be null
                    // Assignment by hardcoded Role ID from test server
                    // await author.AddRoleAsync(author.Guild.GetRole(819999599460483102));
                    
                    // Assignment by hardcoded Role Name
                    await author.AddRoleAsync(author.Guild.Roles.First(sp_role => sp_role.Name == "Birthday Cake"));
            }
        }
    }
}