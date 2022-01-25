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
using BirthdayBot.Extensions;

namespace BirthdayBot.CommandModules
{
    /**
     * This Module contains commands for birthday role assignment and related maintenance
     */
    public class BirthdayModule : ModuleBase<SocketCommandContext>
    {
        private readonly string _roleName; // This will have to be changed once different Role Name for different servers is supported

        private readonly IConfiguration _config;
        private readonly DiscordRestClient _clientRest;
        private readonly RestService _myRest;
        public BirthdayModule(IConfiguration config, DiscordRestClient clientRest, RestService myRest)
        {
            _config = config;
            _clientRest = clientRest;
            _myRest = myRest;

            _roleName = _config["Role Name"]; // _config.GetSection("Role Name").Value.ToString();
        }

        /**
         * Current implementation for Birthday Role assignment is using direct REST call to Discord API
         * circumventing Discord.Net library.
         * Initially was done to avoid requesting Presence Intent for the bot, which is required when
         * using DiscordSocketClient to retrieve GuildUser object.
         * In truth, DiscordRestClient could be used instead, but now that I implemented my own REST call for this -
         * I leave it here, because it is mine and I like it.
         */

        [Command("birthdayme")]
        [Summary("Assign \"Birthday Cake\" role to command caller.")]
        [RequireContext(ContextType.Guild)]
        public async Task AssignBirthdayAsync() // Duplicate code can be removed once AssignBirthdayAsync(string) 
                                                // is adjusted to AssignBirthdayAsync(IUser) (currently won't work)
                
        {
            string userId = Context.User.Id.ToString();
            string guildId = Context.Guild.Id.ToString();
            string roleId = string.Empty;
            try
            {
                roleId = Context.Guild.Roles.First(sp_role => sp_role.Name == _roleName).Id.ToString();
            }
            catch 
            { 
                // If RoleId is not found - it can remain empty; Rest call will fail and throw its own exception
            }

            await _myRest.PutAsync("guilds/" + guildId + "/members/" + userId + "/roles/" + roleId, null);
            }

        [Command("birthday")]
        [Summary("Assign configured birthday role to @mentioned user.\n" +
                "Only the first mentioned user will be processed.")]
        [RequireContext(ContextType.Guild)]
        public async Task AssignBirthdayAsync(string irrelevant) // Not the most graceful circumvention of Discord.NET
                                                                 // IUser TypeReader, which cannot be overriden due to
                                                                 // bug https://github.com/discord-net/Discord.Net/issues/1485
                                                                 // and will not work as it is without Presence Intent;
                                                                 //
                                                                 // I.e.: using real input parameters, for example IUser,
                                                                 // leads to exception throw for any user apart from Bot
                                                                 // and Owner, so I have to use this ugly fix.
        {
            string userId = Context.Message.MentionedUsers.First().Id.ToString();
            string guildId = Context.Guild.Id.ToString();
            string roleId = string.Empty;
            try
            {
                roleId = Context.Guild.Roles.First(sp_role => sp_role.Name == _roleName).Id.ToString();
            }
            catch
            {
                // If RoleId is not found - it can remain empty; Rest call will fail and throw its own exception
            }

            await _myRest.PutAsync("guilds/" + guildId + "/members/" + userId + "/roles/" + roleId, null);
        }

        [Command("birthdaycheck")]
        [Summary("If @mentioned user has a birthday - assign configured birthday role.\n" +
                "Only the first mentioned user will be processed.")]
        [RequireContext(ContextType.Guild)]
        public async Task CheckBirthdayAsync(string irrelevant)
        {
            if (Context.Message.MentionedUsers.Count == 0)
            {
                await ReplyAsync(Context.User.Mention + ", you forgot to @mention a user in your request.");
                return;
            }

            string username = Context.Message.MentionedUsers.First().Username;
            string userId = Context.Message.MentionedUsers.First().Id.ToString();

            string birthday = "";
            try
            {
                birthday = _config
                        .GetSection("Birthdays")
                        .Get<IConfigurationSection[]>()
                        .Select(pairIdBirthday => pairIdBirthday)
                        .Where(pairIdBirthday => pairIdBirthday["Id"] == userId)
                        .First()["Date"];
            }
            catch
            { 
                // Do nothing
            }

            if (string.IsNullOrEmpty(birthday))
            {
                await ReplyAsync("I don't know when " + username + "'s birthday is.");
            }
            else if (birthday == DateTime.Today.ToBirthdayFormat())
            {
                await AssignBirthdayAsync(irrelevant);
                await ReplyAsync(username + "'s birthday is today! Happy Birthday!");
                return;
            }
            else
            {
                await ReplyAsync(username + "'s birthday is not today, it's on " + birthday + "...");
            }
        }

        [Command("guilduser_discordlib")]
        [Summary("Get GuildUser via Discord.NET Rest client")]
        [RequireContext(ContextType.Guild)]
        public async Task GetGuildUserAsync(string irrelevant)
        {
            IGuildUser testUser = await _clientRest.GetGuildUserAsync(Context.Guild.Id, Context.Message.MentionedUsers.First().Id);
            await ReplyAsync(testUser.ToString());

        }


        [Command("birthday_discordlib")]
        [Summary("Assign birthday role to @mentioned user via Discord.NET Rest client")]
        [RequireContext(ContextType.Guild)]
        public async Task AssignRestBirthdayAsync(string irrelevant)
        {
            IGuildUser guildUser = await _clientRest.GetGuildUserAsync(Context.Guild.Id, Context.Message.MentionedUsers.First().Id);
            IRole role = Context.Guild.Roles.First(sp_role => sp_role.Name == _roleName);

            await guildUser.AddRoleAsync(role);
        }

        /**
         * Old implementation relying on Discord.Net library functionality dependent on Presence Intent
         * Kept for reference
         */
        [Command("birthday_deprecated")]
        [Summary("Assign configured birthday role to @mentioned user.")]
        [RequireContext(ContextType.Guild)]
        [Disabled]
        public async Task AssignBirthdayAsync(SocketGuildUser user)
        {
            await user.AddRoleAsync(user.Guild.Roles.First(sp_role => sp_role.Name == _roleName));
        }

        /**
         * Old implementation relying on Discord.Net library functionality dependent on Presence Intent
         * Kept for reference
         */
        [Command("birthday_deprecated")]
        [Summary("Assign configured birthday role to @mentioned user.")]
        [RequireContext(ContextType.Guild)]
        [Disabled]
        public async Task AssignBirthdayAsync(SocketUser user)
        {
            await (user as SocketGuildUser).AddRoleAsync(Context.Guild.Roles.First(sp_role => sp_role.Name == _roleName));
        }

        /**
        * Implementation relying on Discord.Net library functionality dependent on Presence Intent
        * Kept for reference
        */
        [Command("guildusers_deprecated")]
        [Summary("Retrieve a full list of server users.")]
        [RequireContext(ContextType.Guild)]
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