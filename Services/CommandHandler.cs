// .NET Base
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
// Discord
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Rest;
// Internal
using BirthdayBot.BasicModule;
using BirthdayBot.TypeReaders;


namespace BirthdayBot.Services
{
    public class CommandHandler
    {
        // Fields to be set later in the constructor
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly DiscordSocketConfig _clientConfig;
        private readonly DiscordRestClient _restClient;

        public CommandHandler(IServiceProvider services) // This constructor accepts its dependency as an input, which is a type of Dependency Injection
        {
            // Since we passed the services in, we can use GetRequiredService to pass them into the fields set earlier
            // It still expects DiscordSocketClient, so it is not fully abstract -- it merely splits out the implementation from the call
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _clientConfig = services.GetRequiredService<DiscordSocketConfig>();
            _restClient = services.GetRequiredService<DiscordRestClient>();
            _services = services;

            // Take action when we execute a command
            _commands.CommandExecuted += CommandExecutedAsync;

            // Take action when we receive a message (so we can process it, and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // Add TypeReaders to pass non-default arguments to the commands
            // Should implement this outside of CommandHandler unless there's a way to bulk load them like modules
            // _commands.AddTypeReader<IGuildUser>(new GuildUserTypeReader());
            // _commands.AddTypeReader<int>(new TestTypeReader());

            // Confirm added TypeReaders
            //foreach (var x in _commands.TypeReaders)
            //{
            //    Console.WriteLine("[Typereaders] {0}", x.Key);
            //    foreach (TypeReader y in x) 
            //        Console.WriteLine("[Typereaders]   {0}", y);
            //}

            // Potentially reduntant; Experimenting with GuildUser retrieval
            _clientConfig.AlwaysDownloadUsers = true;
            Console.WriteLine(_clientConfig.GatewayIntents.GetValueOrDefault().ToString());

            // Registers commands: all modules that are public and inherit ModuleBase<T>
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        // Takes actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ensures we don't process system messages / messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;

            // Additional check that the message is not system / bot / webhook
            if (message.Source != MessageSource.User) return;

            // Initial value for prefix offset
            var argPos = 0;

            // Gets prefix from the configuration file
            // Accepts empty prefix
            string prefix = _config["Prefix"];

            // Creates context of the received message
            var context = new SocketCommandContext(_client, message);

            // Accept comands both with and without prefix, prioritising prefixed
            // Checks if prefix is Null or Empty
            // -> If YES - ExecuteAsync (Executes command if one is found that matches message context)
            // -> If NO - Determines if the message starts with @mention of the Bot OR has a valid prefix, and adjusts argPos accordingly
            //      -> If YES - ExecuteAsync
            //      -> If NO - ExecuteAsync on argPos 0
            if (String.IsNullOrEmpty(prefix))
                await _commands.ExecuteAsync(context, argPos, _services);
            else if (message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasStringPrefix(prefix, ref argPos))
                await _commands.ExecuteAsync(context, argPos, _services);
            else
            {
                argPos = 0;
                await _commands.ExecuteAsync(context, argPos, _services);
            }
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // If a command isn't found, log that info to console and exit this method
            if (!command.IsSpecified) // && !String.IsNullOrEmpty(_config["Prefix"]))
            {
                System.Console.WriteLine($"Command failed to execute for [{context.User.Username}], error message: [{result.ErrorReason}]");
                return;
            }


            // Log success to the console and exit this method
            if (result.IsSuccess)
            {
                System.Console.WriteLine($"Command [{command.Value.Name}] executed for [{context.User.Username}]");
                return;
            }

            // Remaining scenarios assume Failure; let the user know
            await context.Channel.SendMessageAsync($"Command [{command.Value.Name}] failed for {context.User.Username}, context: [{result}]!");
        }
    }
}