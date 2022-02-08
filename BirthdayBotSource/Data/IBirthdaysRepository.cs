using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BirthdayBot.Data
{
    using UserId = String;
    using ServerId = String;

    public interface IBirthdaysRepository
    {
        public Task AddUserBirthdayAsync(Birthday birthday);

        public Task DeleteUserBirthdayAsync(Birthday birthday);

        public Task AdjustUserBirthdayAsync(Birthday birthday);

        public Task<List<UserId>> LookupUsersByBirthday(DateTime birthdayDate, ServerId serverId = null);
    }
}
