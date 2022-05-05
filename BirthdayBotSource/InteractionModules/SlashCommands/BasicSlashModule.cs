using Discord.Interactions;
using System.Threading.Tasks;

namespace BirthdayBot.InteractionModules
{
    public class BasicSlashModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("beep", "Say hello to the bot.")]
        public async Task HelloBot() =>
            await RespondAsync($"Hello, {Context.User.Mention}!");
    }
}
