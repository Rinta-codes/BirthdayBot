using BirthdayBot.Services;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BirthdayBot.Modules
{
    public class BirthdayModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _config;
        private readonly DiscordRestClient _clientRest;
        private readonly RestService _myRest;
        public BirthdayModule(IConfiguration config, DiscordRestClient clientRest, RestService myRest)
        {
            _config = config;
            _clientRest = clientRest;
            _myRest = myRest;
        }

        [Command("test")]
        public async Task TestAsync(int x) => await ReplyAsync(x.ToString());

        [Command("beep")] // simple interaction
        [Alias("boop")]
        public async Task PingAsync()
            => await ReplyAsync("boop");

        [Command("good bot")] // be polite to your bot
        public async Task GoodBotAsync()
            => await ReplyAsync("thank you");

        [Command("birthdayme")] // Assign hardcoded role to command caller
        public async Task AssignBirthdayAsync()
        {
            await (Context.User as SocketGuildUser).AddRoleAsync(Context.Guild.Roles.First(sp_role => sp_role.Name == "Birthday Cake"));
        }

        // Current implementation using direct REST call to Discord API circumventing Discord.Net library
        [Command("birthday")] // Assign role to @mentioned user by Role Name from configuration
        public async Task AssignBirthday3Async(string irrelevant)
        {
            var roleName = _config["Role Name"];

            string userId = Context.Message.MentionedUsers.First().Id.ToString();
            string guildId = Context.Guild.Id.ToString();
            string roleId = Context.Guild.Roles.First(sp_role => sp_role.Name == roleName).Id.ToString();

            await _myRest.PutAsync("/guilds/" + guildId + "/members/" + userId + "/roles/" + roleId, null);
        }

        // Old implementation relying on Discord.Net library functionality dependent on Presence Intent, will not work most of the times
        [Command("birthday_deprecated")] // Assign role to @mentioned user by Role Name from configuration
        public async Task AssignBirthdayAsync(SocketGuildUser user)
        {
            var roleName = _config["Role Name"];
            await user.AddRoleAsync(user.Guild.Roles.First(sp_role => sp_role.Name == roleName));
        }

        // Old implementation relying on Discord.Net library functionality dependent on Presence Intent, will not work most of the times
        [Command("birthday2_deprecated")] // Assign role to @mentioned user by Role Name from configuration
        public async Task AssignBirthday2Async(SocketUser user)
        {
            var roleName = _config["Role Name"];
            await (user as SocketGuildUser).AddRoleAsync(Context.Guild.Roles.First(sp_role => sp_role.Name == roleName));
        }

        [Command("guildusers")] // Retrieve a full list of server users
        public async Task GetGuildUsersAsync()
        {
            string response = "";

            // Implementation via Guild.Users
            // foreach (var x in Context.Guild.Users) { response += (x.Username + " "); };

            //Implementation via REST Client directly
            //RestGuild guildRest = await _clientRest.GetGuildAsync(Context.Guild.Id);
            //await guildRest.GetUsersAsync().ForEachAsync(x =>
            //{
            //    foreach (IGuildUser y in x) { Console.Write("{name} ", y.Username); };
            //});

            // Implementation via Websocket method that uses REST API
            await Context.Guild.GetUsersAsync().ForEachAsync(x =>
            {
                foreach (IGuildUser y in x) { response += y.Username + " "; };
            });

            // Sending response via REST for test purposes
            RestGuild guildRest = await _clientRest.GetGuildAsync(Context.Guild.Id);
            RestTextChannel channelRest = await guildRest.GetTextChannelAsync(Context.Channel.Id);
            await channelRest.SendMessageAsync(response);

            // Sending response via WebSocket
            // await ReplyAsync(response);
        }
    }
}