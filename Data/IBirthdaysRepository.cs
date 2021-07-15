using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BirthdayBot.Data
{
    using UserId = String;

    public interface IBirthdaysRepository
    {
        public Task AddUserBirthdayAsync(string userId, DateTime birthdayDate);

        public Task DeleteUserBirthdayAsync(string userId);

        public Task AdjustUserBirthdayAsync(string userId, DateTime newBirthdayDate);

        public Task<List<UserId>> LookupUsersByBirthday(DateTime birthdayDate);
    }
}
