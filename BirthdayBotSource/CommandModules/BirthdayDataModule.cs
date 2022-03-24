using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using BirthdayBot.Data;
using BirthdayBot.Extensions;
using System;

namespace BirthdayBot.CommandModules
{
    public class BirthdayDataModule : ModuleBase<SocketCommandContext>
    {
        private readonly IBirthdaysRepository _birthdays;

        public BirthdayDataModule (IBirthdaysRepository birthdays)
        {
            _birthdays = birthdays;
        }

        [Command("birthdayadd")]
        [Summary("Add new user birthday")]
        [RequireContext(ContextType.Guild)]
        public async Task AddBirthdayAsync(SocketUser socketUser, string date)
        {
            date.FromBirthdayFormat(out DateTime birthdayDateTime);
            await _birthdays.AddUserBirthdayAsync(new Birthday(socketUser.Id.ToString(), birthdayDateTime));
        }
    }
}
