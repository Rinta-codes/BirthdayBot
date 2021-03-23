// .NET Base
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
// Discord
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Rest;
// Internal
using BirthdayBot.Services;


// Initialises the Bot, connects it to Discord, and handles it over to Command Handler service 
namespace BirthdayBot
{
    class BirthdayBot
    {
        // Configuration path
        private static string _configPath = "config.json";

        // Prefix for log messages by source
        private static string _wsLogPrefix = "[WebSocket]";
        private static string _restLogPrefix = "[REST]";
        private static string _csLogPrefix = "[CommandService]";

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

                // Hook into log events for client and commands
                client.Log += (LogMessage log) => BirthdayBot.LogAsync(log, _wsLogPrefix);
                restClient.Log += (LogMessage log) => BirthdayBot.LogAsync(log, _restLogPrefix);
                services.GetRequiredService<CommandService>().Log += (LogMessage log) => BirthdayBot.LogAsync(log, _csLogPrefix);

                // Hook into the client ready event
                client.Ready += () => BirthdayBot.ReadyAsync(_wsLogPrefix);
                restClient.LoggedIn += () => BirthdayBot.ReadyAsync(_restLogPrefix);

                // Get the Token value from the configuration file
                await client.LoginAsync(TokenType.Bot, config["Token"]);
                await restClient.LoginAsync(TokenType.Bot, config["Token"]);

                // Start up the WebSocket connection
                // Will immediately return after being called, initialising the connection on another thread
                await client.StartAsync();
                
                // Start up CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                // Block the program until it is closed, so that Bot keeps running after connecting
                await Task.Delay(-1);
            }
        }

        // Load configuration from a json file
        private static IConfiguration GetConfig(string configPath)
        {
            return new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(path: configPath).Build();
        }

        // Print log message into console, preceeded by prefix string
        private static Task LogAsync(LogMessage log, string prefix)
        {
            Console.WriteLine("{0} {1}", prefix, log.ToString());
            return Task.CompletedTask;
        }

        // Print connection confirmation into console, preceeded by prefix string
        private static Task ReadyAsync(string prefix)
        {
            Console.WriteLine("{0} Connected...", prefix);
            return Task.CompletedTask;
        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<IConfiguration>(BirthdayBot.GetConfig(_configPath))
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<DiscordSocketConfig>()
                .AddSingleton<DiscordRestClient>()
 //               .AddHttpClient()
                .AddSingleton<RestService>()
                .BuildServiceProvider();
        }
    }
}