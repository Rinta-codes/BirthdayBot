// .NET Base
using System.Threading.Tasks;
// Discord
using Discord;
using Discord.Commands;
// Internal
using BirthdayBot.Services;

namespace BirthdayBot.BasicModule
{
    public class BasicModule : ModuleBase<SocketCommandContext>
    { 
        [Command("beep")]
        [Alias("boop")]
        public Task PingAsync()
            => ReplyAsync("boop");

        [Command("good bot")]
        public Task GoodBotAsync()
            => ReplyAsync("thank you");
    }
}