// .NET Base
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
// Discord
using Discord;
using Discord.Commands;
using Discord.WebSocket;
// Internal
using BirthdayBot.Services;

namespace BirthdayBot.BasicModule
{
    public class BasicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _config;
        public BasicModule(IConfiguration config)
        {
            _config = config;
        }


        [Command("beep")]
        [Alias("boop")]
        public async Task PingAsync()
            => await ReplyAsync("boop");

        [Command("good bot")]
        public async Task GoodBotAsync()
            => await ReplyAsync("thank you");

        [Command("birthdayme")]
        public async Task AssignBirthdayAsync()
        {
            // Resolve<BusSettings>()
            await (Context.User as SocketGuildUser).AddRoleAsync(Context.Guild.Roles.First(sp_role => sp_role.Name == "Birthday Cake"));
        }

        [Command("birthday")]
        public async Task AssignBirthdayAsync(SocketGuildUser user)
        {
            var roleName = _config["Role Name"];
            await user.AddRoleAsync(user.Guild.Roles.First(sp_role => sp_role.Name == roleName));
        }
    }
}