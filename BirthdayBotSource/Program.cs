﻿using BirthdayBot.Configuration;
using BirthdayBot.Data;
using BirthdayBot.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

/** 
 * Initialises the Bot, connects it to Discord, and hands it over to Command Handler service 
 */
namespace BirthdayBot
{
    public class Constants
    {
        public static readonly string _birthdayDateFormat = "dd MMM";
    }

    public class BirthdayBot
    {
        // Configuration
        private static readonly string _configPath = "config.json";
        private static readonly string _birthdaysPath = "birthdays.json";
        private static readonly IConfiguration _config = GetConfig(_configPath);

        // Prefix for log messages by source
        private static readonly string _socketLogPrefix = "[WebSocket]";
        private static readonly string _restLogPrefix = "[REST]";
        private static readonly string _csLogPrefix = "[CommandService]";

        static async Task Main()
        {
            // "using" statement facilitates correct handling of IDisposable object such as ServiceProvider
            using var services = ConfigureServices();

            var socketClient = services.GetRequiredService<DiscordSocketClient>();
            var restClient = services.GetRequiredService<DiscordRestClient>();
            var commandService = services.GetRequiredService<CommandService>();
            var connectionConfig = services.GetRequiredService<IOptions<ConnectionConfiguration>>();
            var birthdays = services.GetRequiredService<IBirthdaysRepository>();

            // Hook into log events for clients and CommandService
            socketClient.Log += (LogMessage message) => BirthdayBot.LogAsync(message, _socketLogPrefix);
            restClient.Log += (LogMessage message) => BirthdayBot.LogAsync(message, _restLogPrefix);
            commandService.Log += (LogMessage message) => BirthdayBot.LogAsync(message, _csLogPrefix);

            // Hook into clients ready events
            socketClient.Ready += () => BirthdayBot.ReadyAsync(_socketLogPrefix);
            restClient.LoggedIn += () => BirthdayBot.ReadyAsync(_restLogPrefix);

            // Configure login details for the clients
            await socketClient.LoginAsync(TokenType.Bot, connectionConfig.Value.Token);
            await restClient.LoginAsync(TokenType.Bot, connectionConfig.Value.Token);

            // Start up the WebSocket connection
            // Will immediately return after being called, initialising the connection on another thread
            await socketClient.StartAsync();

            // Load birthdays data into cache, if it is required
            if (birthdays is IBirthdaysRepositoryCached birthdaysWithCache)
                await birthdaysWithCache.LoadFromSourceAsync();

            // Initialize CommandHandler and ActionHandler services
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            await services.GetRequiredService<ActionHandlingService>().InitializeAsync();
            await services.GetRequiredService<InteractionHandlingService>().InitializeAsync();

            await Task.Delay(-1);

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
        private static Task LogAsync(LogMessage message, string prefix)
        {
            Console.WriteLine($"{prefix} {message.ToString()}");
            return Task.CompletedTask;
        }

        /**
         * Prints connection confirmation into console, preceeded by prefix string
         */
        private static Task ReadyAsync(string prefix)
        {
            Console.WriteLine($"{prefix} Ready");
            return Task.CompletedTask;
        }

        /**
         * Assembles all required services and their dependencies via DI container
         */
        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<IConfiguration>(GetConfig(_birthdaysPath))
                .AddSingleton<Stream>(new FileStream(_birthdaysPath, FileMode.Open))
                .AddOptions()
                    .Configure<ConnectionConfiguration>(connectionConfiguration => _config.Bind(connectionConfiguration))
                    .Configure<CommandsConfiguration>(commandsConfiguration => _config.Bind(commandsConfiguration))
                    .Configure<BirthdayConfiguration>(birthdayConfiguration => _config.Bind(birthdayConfiguration))
                .AddSingleton<IBirthdaysRepository, BirthdaysRepositoryFromJson<BirthdaysCacheMemory>>()
                .AddSingleton<DiscordRestClient>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<DiscordSocketConfig>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddHttpClient("RestClient", (services, client) =>
                 {
                     client.BaseAddress = new Uri("https://discord.com/api/");
                     client.DefaultRequestHeaders.Clear();
                     client.DefaultRequestHeaders.Add("Authorization", "Bot" + " " + services.GetRequiredService<IOptions<ConnectionConfiguration>>().Value.Token);
                     // TBU for header ("Content-Type", "application/json") // MediaTypeWithQualityHeaderValue("application/json")
                 }).Services
                .AddSingleton<RestService>()
                .AddSingleton<TimerFactory>()
                .AddSingleton<ActionHandlingService>()
                .AddSingleton<InteractionService>(services => new InteractionService(services.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandlingService>()
                .BuildServiceProvider();
        }
    }
}