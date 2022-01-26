using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayBot.Data
{
    using UserId = String;

    /*
     * This class implements IBirthdaysRepository interface
     * when birthday data is stored in database
     * without use of cache
     */
    class BirthdaysRepositoryDatabase : IBirthdaysRepository
    {
        public async Task AddUserBirthdayAsync(string userId, DateTime birthdayDate)
        { }

        public async Task DeleteUserBirthdayAsync(string userId)
        { }

        public async Task AdjustUserBirthdayAsync(string userId, DateTime newBirthdayDate)
        { }

        public async Task<List<UserId>> LookupUsersByBirthday(DateTime birthdayDate)
        {
            List<UserId> users = new();

            return users;
        }

    }
}
