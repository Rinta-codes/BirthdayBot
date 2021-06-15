using BirthdayBot.Data;
using BirthdayBot.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BirthdayBot.ActionModules
{
    /*
     * Stores Actions to be executed by Bot on its own
     */
    public class ActionModule
    {
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _client;
        private readonly RestService _myRest;
        private readonly BirthdaysRepository _birthdays;
        public ActionModule(IConfiguration config, DiscordSocketClient client, RestService myRest, BirthdaysRepository birthdays)
        {
            _config = config;
            _client = client;
            _myRest = myRest;
            _birthdays = birthdays;
        }

        /*
         * Check if any of the birthdays are today.
         * For all discovered birthdays:
         *  - Assign Birthday Role to corresponding user in all applicable servers.
         *  - Send a congratulatory message to a default channel of each applicable server.
         */
        public async Task SetBirthdaysActionAsync()
        {
            Console.WriteLine("[SetBirthdaysAction] Execution has began.");

            List<string> todaysBirthdays = new(_birthdays.LookupUsersByBirthday(DateTime.Today));

            if (todaysBirthdays.Count() == 0)
            {
                Console.WriteLine("[SetBirthdaysAction] Execution completed. No birthdays detected.");
                return;
            }

            var guilds = await (_client as IDiscordClient).GetGuildsAsync();
            var roleName = _config.GetSection("Role Name").Value.ToString();
            string roleId;
            SocketTextChannel defaultChannel;
            SocketUser user;

            foreach (string userId in todaysBirthdays)
            {
                foreach (var guild in guilds)
                {
                    roleId = guild.Roles.First(sp_role => sp_role.Name == roleName).Id.ToString();
                    defaultChannel = await guild.GetDefaultChannelAsync() as SocketTextChannel;
                    user = (await (_client as IDiscordClient).GetUserAsync(ulong.Parse(userId)) as SocketUser);

                    // Add role (by id) to the user (by id) - requires no IRole or IUser objects
                    await _myRest.PutAsync("/guilds/" + guild.Id + "/members/" + userId + "/roles/" + roleId, null);

                    if (user is not null)
                        // Username can be replaced with Nickname once I change user type from SocketUser to GuildUser
                        await defaultChannel.SendMessageAsync("It's " + user.Username + "'s birthday today! Happy Birthday!"); // <- fails to retrieve some of the users on the first try
                    else
                        Console.WriteLine($"No user retrieved for user id = {userId}");
                }
            }

            Console.WriteLine("[{0}] [SetBirthdaysAction] Execution completed. {1} birthdays detected.", DateTime.Now.ToString(), todaysBirthdays.Count());
        }

    }
}
