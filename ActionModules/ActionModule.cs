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
    class ActionModule
    {
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _client;
        private readonly RestService _myRest;
        public ActionModule(IConfiguration config, DiscordSocketClient client, RestService myRest)
        {
            _config = config;
            _client = client;
            _myRest = myRest;
        }

        /*
         * Check if any of the birthdays in the config are today.
         * For all discovered birthdays:
         *  - Assign Birthday Role to corresponding user in all applicable servers.
         *  - Send a congratulatory message to a default channel of each applicable server.
         */
        public async Task SetBirthdaysAction()
        {
            Console.WriteLine("[SetBirthdaysAction] Execution has began.");

            List<string> todaysBirthdays = new();

            foreach (var pairIdBirthday in _config.GetSection("Birthdays").Get<IConfigurationSection[]>())
            {
                if (pairIdBirthday["Date"] == DateTime.Today.ToString("dd MMM"))
                {
                    todaysBirthdays.Add(pairIdBirthday["Id"]);
                }
            }

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
                    await _myRest.PutAsync("/guilds/" + guild.Id + "/members/" + userId + "/roles/" + roleId, null);
                    defaultChannel = await guild.GetDefaultChannelAsync() as SocketTextChannel;
                    user = (await (_client as IDiscordClient).GetUserAsync(ulong.Parse(userId)) as SocketUser);
                    await defaultChannel.SendMessageAsync("It's " + user.Username + "'s birthday today! Happy Birthday!");
                }
            }

            Console.WriteLine("[{0}] [SetBirthdaysAction] Execution completed. {1} birthdays detected.", DateTime.Now.ToString(), todaysBirthdays.Count());
        }

    }
}
