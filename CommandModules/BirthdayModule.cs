using BirthdayBot.Preconditions;
using BirthdayBot.Services;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BirthdayBot.CommandModules
{
    /**
     * This Module contains commands for birthday role assignment and related maintenance
     */
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

        [Command("birthdayme")]
        [Summary("Assign \"Birthday Cake\" role to command caller.")]
        [RequireGuild]
        public async Task AssignBirthdayAsync()
        {
            await (Context.User as SocketGuildUser).AddRoleAsync(Context.Guild.Roles.First(sp_role => sp_role.Name == "Birthday Cake"));
        }

        /**
         * Current implementation is using direct REST call to Discord API circumventing Discord.Net library
         */
        [Command("birthday")]
        [Summary("Assign configured birthday role to @mentioned user.\n" +
                "Only the first mentioned user will be processed.")]
        [RequireGuild]
        public async Task AssignBirthdayAsync(string irrelevant) // Not the most graceful circumvention of Discord.NET
                                                                 // IUser TypeReader, which cannot be overriden due to
                                                                 // bug https://github.com/discord-net/Discord.Net/issues/1485
                                                                 // and will not work as it is without Presence Intent
        {
            // var roleName = _config["Role Name"];
            var roleName = _config.GetSection("Role Name").Value.ToString();

            string userId = Context.Message.MentionedUsers.First().Id.ToString();
            string guildId = Context.Guild.Id.ToString();
            string roleId = Context.Guild.Roles.First(sp_role => sp_role.Name == roleName).Id.ToString();

            await _myRest.PutAsync("guilds/" + guildId + "/members/" + userId + "/roles/" + roleId, null);
        }

        [Command("birthdaycheck")]
        [Summary("If @mentioned user has a birthday - assign configured birthday role.\n" +
                "Only the first mentioned user will be processed.")]
        [RequireGuild]
        public async Task CheckBirthdayAsync(string irrelevant)
        {
            string birthday = "";
            foreach (var pairIdBirthday in _config.GetSection("Birthdays").Get<IConfigurationSection[]>())
            {
                if (pairIdBirthday["Id"] == Context.Message.MentionedUsers.First().Id.ToString())
                {
                    birthday = pairIdBirthday["Date"];
                }
            }

            if (birthday == DateTime.Today.ToString("dd MMM"))
            {
                await AssignBirthdayAsync(irrelevant);
                await ReplyAsync(Context.Message.MentionedUsers.First().Username + "'s birthday is today! Happy Birthday!");
                return;
            }

            await ReplyAsync(Context.Message.MentionedUsers.First().Username + "'s birthday is not today, it's on " + birthday + "...");
        }

        [Command("guilduser")]
        [Summary("Get GuildUser via Rest")]
        [RequireGuild]
        public async Task GetGuildUserAsync(string irrelevant)
        {
            IGuildUser testUser = await _clientRest.GetGuildUserAsync(Context.Guild.Id, Context.Message.MentionedUsers.First().Id);
            await ReplyAsync(testUser.ToString());

        }


        [Command("birthdayrest")]
        [Summary("Get GuildUser via Rest")]
        [RequireGuild]
        public async Task AssignRestBirthdayAsync(string irrelevant)
        {
            IGuildUser guildUser = await _clientRest.GetGuildUserAsync(Context.Guild.Id, Context.Message.MentionedUsers.First().Id);
            var roleName = _config.GetSection("Role Name").Value.ToString();
            IRole role = Context.Guild.Roles.First(sp_role => sp_role.Name == roleName);

            await guildUser.AddRoleAsync(role);
        }

        /**
         * Old implementation relying on Discord.Net library functionality dependent on Presence Intent
         * Kept for reference
         */
        [Command("birthday_deprecated")]
        [Summary("Assign configured birthday role to @mentioned user.")]
        [RequireGuild]
        [Disabled]
        public async Task AssignBirthdayAsync(SocketGuildUser user)
        {
            var roleName = _config["Role Name"];
            await user.AddRoleAsync(user.Guild.Roles.First(sp_role => sp_role.Name == roleName));
        }

        /**
         * Old implementation relying on Discord.Net library functionality dependent on Presence Intent
         * Kept for reference
         */
        [Command("birthday_deprecated")]
        [Summary("Assign configured birthday role to @mentioned user.")]
        [RequireGuild]
        [Disabled]
        public async Task AssignBirthdayAsync(SocketUser user)
        {
            var roleName = _config["Role Name"];
            await (user as SocketGuildUser).AddRoleAsync(Context.Guild.Roles.First(sp_role => sp_role.Name == roleName));
        }

        /**
        * Implementation relying on Discord.Net library functionality dependent on Presence Intent
        * Kept for reference
        */
        [Command("guildusers_deprecated")]
        [Summary("Retrieve a full list of server users.")]
        [RequireGuild]
        [Disabled]
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