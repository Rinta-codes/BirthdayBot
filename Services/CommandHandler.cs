using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BirthdayBot.Services
{
    /**
     * Listens to received messages to catch commands, sends commands over to Command Service & processes subsequent Command Service output
     */
    public class CommandHandler
    {
        // Fields to be set later in the constructor
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        // private readonly DiscordSocketConfig _clientConfig; // Not needed at the moment
        // private readonly DiscordRestClient _restClient; // Not needed at the moment

        public CommandHandler(IServiceProvider services)
        {
            _services = services;
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            // _clientConfig = services.GetRequiredService<DiscordSocketConfig>(); // Not needed at the moment
            // _restClient = services.GetRequiredService<DiscordRestClient>(); // Not needed at the moment

            // Hook into CommandExecuted event to print out / log command execution result
            _commands.CommandExecuted += CommandExecutedAsync;

            // Hook into MessageReceived event to execute commands received as messages
            _client.MessageReceived += MessageReceivedAsync;
        }

        /**
         * Async part of CommandHandler initialisation 
         */
        public async Task InitializeAsync()
        {
            // Add TypeReaders to pass non-default arguments to the commands // Not needed at the moment
            // Should implement this outside of CommandHandler unless there's a way to bulk load them like modules
            // _commands.AddTypeReader<IGuildUser>(new GuildUserTypeReader());
            // _commands.AddTypeReader<int>(new TestTypeReader());

            // Confirm added TypeReaders // Not needed at the moment
            // foreach (var x in _commands.TypeReaders)
            // {
            //     Console.WriteLine("[Typereaders] {0}", x.Key);
            //     foreach (TypeReader y in x) 
            //         Console.WriteLine("[Typereaders]   {0}", y);
            // }

            // Registers commands: all modules that are public and inherit ModuleBase<T>

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        // Take actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ensures we don't process system messages / messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;

            // Additional check that the message is not system / bot / webhook
            if (message.Source != MessageSource.User) return;

            // Initial value for prefix offset
            var prefixOffset = 0;

            // Gets prefix from the configuration file
            // Accepts empty prefix
            string prefix = _config["Prefix"];

            // Creates context of the received message
            var context = new SocketCommandContext(_client, message);

            // Checks if Prefix is null or empty
            // If not - checks for prefix at the start of received message and adjusts offset accordingly
            if (String.IsNullOrEmpty(prefix)) { }
            else message.HasStringPrefix(prefix, ref prefixOffset);

            // Executes command if one is found that matches message context
            await _commands.ExecuteAsync(context, prefixOffset, _services);
        }

        /**
         * Handles result of command execution, including when command was not found
         */
        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // If a command isn't found, log that info to console and exit this method
            // Right now will log for every message without command
            if (!command.IsSpecified)
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
            await context.Channel.SendMessageAsync($"Command [{command.Value.Name}] failed for {context.User.Username}, context: [{result}]");
        }
    }
}