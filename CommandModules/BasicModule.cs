using BirthdayBot.Preconditions;
using Discord.Commands;
using System.Threading.Tasks;


namespace BirthdayBot.CommandModules
{
    /**
     * This Module contains basic commands for simple bot interactions
     */
    public class BasicModule : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        [Summary("Test command for int TypeReader override.")]
        [Disabled]
        public async Task TestAsync(int x) => await ReplyAsync(x.ToString());

        [Command("beep")]
        [Alias("boop")]
        [Summary("Simple interaction to test that the Bot is up.")]
        public async Task PingAsync()
            => await ReplyAsync("boop");

        [Command("good bot")]
        [Summary("Be polite to your Bot.")]
        public async Task GoodBotAsync()
            => await ReplyAsync("Thank you!");


        [Command("thank you bot")]
        [Summary("Say \"thank you\" to your Bot.")]
        public async Task ThanksBotAsync()
            => await ReplyAsync("You are welcome!");
    }
}
