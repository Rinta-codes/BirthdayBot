using BirthdayBot.Preconditions;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace BirthdayBot.CommandModules
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

            foreach (var command in _commands.Commands)
            {
                if (command.CheckPreconditionsAsync(Context).Result.IsSuccess)
                {
                    embed.AddField(string.Join(", ", command.Aliases), command.Summary);
                }
            }

            // Note:
            // https://discord.com/developers/docs/resources/channel#embed-limits
            // title        256 characters
            // description  4096 characters
            // fields       Up to 25 field objects
            // field.name   256 characters
            // field.value  1024 characters
            // footer.text  2048 characters
            // author.name  256 characters

            // For now I will not handle the above, but I might return to it later

            await ReplyAsync(embed: embed.Build());
        }

    }
}
