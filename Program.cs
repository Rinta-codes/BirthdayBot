using BirthdayBot.Services;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

/** 
 * Initialises the Bot, connects it to Discord, and hands it over to Command Handler service 
 */
namespace BirthdayBot
{
    class BirthdayBot
    {
        // Configuration path
        private static readonly string _configPath = "config.json";

        // Prefix for log messages by source
        private static readonly string _socketLogPrefix = "[WebSocket]";
        private static readonly string _restLogPrefix = "[REST]";
        private static readonly string _csLogPrefix = "[CommandService]";

        static async Task Main()
        {
            /**
             * Correct way of handling IDisposable object
             */
            using (var services = BirthdayBot.ConfigureServices())
            {
                // Temporary variables for clients and config, since these are being called multiple times
                var socketClient = services.GetRequiredService<DiscordSocketClient>();
                var restClient = services.GetRequiredService<DiscordRestClient>();
                var config = services.GetRequiredService<IConfiguration>();

                // Hook into log events for clients and CommandService
                socketClient.Log += (LogMessage log) => BirthdayBot.LogAsync(log, _socketLogPrefix);
                restClient.Log += (LogMessage log) => BirthdayBot.LogAsync(log, _restLogPrefix);
                services.GetRequiredService<CommandService>().Log += (LogMessage log) => BirthdayBot.LogAsync(log, _csLogPrefix);

                // Hook into the clients ready events
                socketClient.Ready += () => BirthdayBot.ReadyAsync(_socketLogPrefix);
                restClient.LoggedIn += () => BirthdayBot.ReadyAsync(_restLogPrefix);

                // Configure login details for the clients
                await socketClient.LoginAsync(TokenType.Bot, config["Token"]);
                await restClient.LoginAsync(TokenType.Bot, config["Token"]);

                // Start up the WebSocket connection
                // Will immediately return after being called, initialising the connection on another thread
                await socketClient.StartAsync();

                // Initialize CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();
                await services.GetRequiredService<ActionHandler>().Initialize();

                // Block the program until it is closed, so that Bot keeps running after connecting
                await Task.Delay(-1);
            }
        }

        /**
         * Loads configuration from a json file
         */
        private static IConfiguration GetConfig(string configPath)
        {
            return new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(path: configPath).Build();
        }

        /**
         * Prints log message into console, preceeded by prefix string
         */
        private static Task LogAsync(LogMessage log, string prefix)
        {
            Console.WriteLine("{0} {1}", prefix, log.ToString());
            return Task.CompletedTask;
        }

        /**
         * Prints connection confirmation into console, preceeded by prefix string
         */
        private static Task ReadyAsync(string prefix)
        {
            Console.WriteLine("{0} Ready", prefix);
            return Task.CompletedTask;
        }

        /**
         * Assembles all required services and their dependencies via DI container
         */
        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<IConfiguration>(BirthdayBot.GetConfig(_configPath))
                .AddSingleton<DiscordRestClient>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<DiscordSocketConfig>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddHttpClient()
                .AddSingleton<RestService>()
                .AddSingleton<TimerFactory>()
                .AddSingleton<ActionHandler>()
                .BuildServiceProvider();
        }
    }
}