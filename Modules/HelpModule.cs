using BirthdayBot.Services;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BirthdayBot.Modules
{
    /**
     * This Module contains Help commands
     */
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;

        public HelpModule(CommandService commands)
        {
            _commands = commands;
        }

        [Command("help")]
        [Summary("Lists available commands and their descriptions.\n" +
            "Ignores disabled commands.")]
        public async Task CommandListAsync()
        {

            var embed = new EmbedBuilder()
                .WithAuthor(Context.Client.CurrentUser)
                .WithTitle("__Available Commands__")
                .WithColor(Color.Magenta);
            

            bool disabled = false;
            foreach (var command in _commands.Commands)
            {
                foreach (var precondition in command.Preconditions)
                    if (precondition is Disabled)
                        disabled = true;

                if (!disabled)
                {
                    string aliases = new(string.Empty);
                    foreach (var alias in command.Aliases)
                        aliases += alias + ", ";
                    embed.AddField(aliases.Remove(aliases.Length - 2), command.Summary);
                }

                disabled = false;
            }

            // await Context.User.SendMessageAsync(embed: embed.Build());
            await ReplyAsync(embed: embed.Build());
        }

    }
}
