// .NET Base
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
// Discord
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Rest;
// Internal
using BirthdayBot.Services;

namespace BirthdayBot.BasicModule
{
    public class BasicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _config;
        private readonly DiscordRestClient _clientRest;
        public BasicModule(IConfiguration config, DiscordRestClient clientRest)
        {
            _config = config;
            _clientRest = clientRest;
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
            // Resolve<BusSettings>()
            await (Context.User as SocketGuildUser).AddRoleAsync(Context.Guild.Roles.First(sp_role => sp_role.Name == "Birthday Cake"));
        }

        [Command("birthday")] // Assign role to @mentioned user by Role Name from configuration
        public async Task AssignBirthdayAsync(SocketGuildUser user)
        {
            var roleName = _config["Role Name"];
            await user.AddRoleAsync(user.Guild.Roles.First(sp_role => sp_role.Name == roleName));
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