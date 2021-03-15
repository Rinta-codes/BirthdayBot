// .NET Base
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
// Discord
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
// Internal
using BirthdayBot.Services;

namespace BirthdayBot
{
    class BirthdayBot
    {
        static async Task Main()
        {
            // Call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using (var services = BirthdayBot.ConfigureServices())
            {
                // Temporary variables for client and config
                // Makes for neat code and useful if we need to access them few times
                var client = services.GetRequiredService<DiscordSocketClient>();
                var config = services.GetRequiredService<IConfiguration>();

                // Hook into log events for client and commands and write it out to the console
                client.Log += BirthdayBot.LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // Hook into the client ready event
                client.Ready += BirthdayBot.ReadyAsync;
                
                // Get the Token value from the configuration file
                await client.LoginAsync(TokenType.Bot, config["Token"]);

                // Start up the connection
                // Will immediately return after being called, initialising the connection on another thread
                await client.StartAsync();

                // Start up CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                // Block the program until it is closed, so that Bot keeps running after connecting
                await Task.Delay(-1);
            }
        }

        // Load configuration from a file
        private static IConfiguration GetConfig()
        {
            return new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(path: "config.json").Build();
        }

        private static Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private static Task ReadyAsync()
        {
            Console.WriteLine($"Connected...");
            return Task.CompletedTask;
        }

        /* OLD MESSAGE HANDLING CODE
         
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

        */

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(BirthdayBot.GetConfig())
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}