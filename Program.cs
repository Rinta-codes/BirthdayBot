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
using Discord.Rest;
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
                var restClient = services.GetRequiredService<DiscordRestClient>();
                var config = services.GetRequiredService<IConfiguration>();

                // Hook into log events for client and commands and write it out to the console
                client.Log += (LogMessage log) => BirthdayBot.LogAsync(log, "[WebSocket]");
                restClient.Log += (LogMessage log) => BirthdayBot.LogAsync(log, "[REST]");
                services.GetRequiredService<CommandService>().Log += (LogMessage log) => BirthdayBot.LogAsync(log, "[CommandService]");

                // Hook into the client ready event
                client.Ready += () => BirthdayBot.ReadyAsync("[WebSocket]");
                restClient.LoggedIn += () => BirthdayBot.ReadyAsync("[REST]");

                // Get the Token value from the configuration file
                await client.LoginAsync(TokenType.Bot, config["Token"]);
                await restClient.LoginAsync(TokenType.Bot, config["Token"]);

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

        private static Task LogAsync(LogMessage log, string prefix)
        {
            Console.WriteLine("{0} {1}", prefix, log.ToString());
            return Task.CompletedTask;
        }

        private static Task ReadyAsync(string prefix)
        {
            Console.WriteLine("{0} Connected...", prefix);
            return Task.CompletedTask;
        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(BirthdayBot.GetConfig())
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<DiscordSocketConfig>()
                .AddSingleton<DiscordRestClient>()
                .BuildServiceProvider();
        }
    }
}