﻿// .NET Base
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
// Discord
using Discord;
using Discord.Commands;
using Discord.WebSocket;
// Internal
using BirthdayBot.BasicModule;


namespace BirthdayBot.Services
{
    public class CommandHandler
    {
        // Fields to be set later in the constructor
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services) // This constructor accepts its dependency as an input, which is a type of Dependency Injection
        {
            // Since we passed the services in, we can use GetRequiredService to pass them into the fields set earlier
            // It still expects DiscordSocketClient, so it is not fully abstract -- it merely splits out the implementation from the call
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // Take action when we execute a command
            _commands.CommandExecuted += CommandExecutedAsync;

            // Take action when we receive a message (so we can process it, and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync() // Commands are modular (i.e. plugins) and this method loads them up
        {
            // Registers modules that are public and inherit ModuleBase<T>
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        // Takes actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ensures we don't process system messages / messages from other bots received via WebSocket
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }

            // Ensures we don't process system messages / messages from other bots received via other means
            if (message.Source != MessageSource.User)
            {
                return;
            }

            // Sets the argument position away from the prefix we set
            var argPos = 0;

            // Gets prefix from the configuration file
            // Accounts for possibility of null or multicharacter prefix -> will not be applied
            try
            {
                char prefix = Char.Parse(_config["Prefix"]);

                // Determines if the message starts with @mention of the Bot OR has a valid prefix, and adjusts argPos accordingly; exits if neither is true
                if (!message.HasMentionPrefix(_client.CurrentUser, ref argPos) || !message.HasCharPrefix(prefix, ref argPos))
                {
                    return;
                }
            }
            catch (Exception e)
            {
                if (e is FormatException)
                    Console.WriteLine("Warning: Multi-character prefix was specified in configuration - Prefix is disregarded!");

                // Determines if the message starts with @mention of the Bot
                if (!message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    return;
                }
            }
            
            // If we are still here - creates context of the received message
            var context = new SocketCommandContext(_client, message);

            // Executes command if one is found that matches message context
            // If multiple commands are found - Exception will be thrown; This behavior can be changed via adding another argument to the method call
            // Presumably Exceptions are handled in CommandHandler.CommandExecutedAsync
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // If a command isn't found, log that info to console and exit this method
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

            // Failure scenario, let the user know
            await context.Channel.SendMessageAsync($"Command [{command.Value.Name}] failed for {context.User.Username}, context: [{result}]!");
        }
    }
}