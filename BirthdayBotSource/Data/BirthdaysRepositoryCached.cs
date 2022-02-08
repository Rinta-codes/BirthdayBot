using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BirthdayBot.Data
{
    using UserId = String;
    using ServerId = String;

    /*
     * This abstract class implements cache storage of birthday data 
     * along with part of the interface that will have to interact with cache.
     * 
     * Various ways of loading initial data (from config, from database, etc.) 
     * and saving changed data are supplied by concrete classes derived from this one.
     */
    public abstract class BirthdaysRepositoryCached : IBirthdaysRepositoryCached
    {
        // Since I will need to look up Users from Birthdays, which is a
        // many-to-one relation, List makes more sense than a Dictionary
        private List<Birthday> _birthdaysCache = new();

        // Separately stored list of User IDs for fast duplicate check
        private HashSet<string> userIdsCache = new();

        private bool IsUserDuplicate(string userId)
        {
            if (userIdsCache.Contains(userId))
                return true;
            else
                return false;
        }

        protected void AddUserBirthdayInternalStorage(Birthday birthday)
        {
            _birthdaysCache.Add(birthday);
            userIdsCache.Add(birthday.UserId);
        }

        public async Task AddUserBirthdayAsync(Birthday birthday)
        {
            if (IsUserDuplicate(birthday.UserId))
                throw new ArgumentException($"Failed to add a new User Birthday. The following UserId already exists: {birthday.UserId}");
            else
            {
                AddUserBirthdayInternalStorage(birthday);
                await SaveChangesAsync();
            }
        }

        public async Task DeleteUserBirthdayAsync(Birthday birthday) // TBU
        {
            // TBU
        }

        public async Task AdjustUserBirthdayAsync(Birthday birthday) // TBU
        {
            // TBU
        }

        public async Task<List<UserId>> LookupUsersByBirthday(DateTime birthdayDate, ServerId serverId = null)
        {
            List<UserId> users = _birthdaysCache
                .Where(birthday => birthday.BirthdayDate.Date == birthdayDate.Date
                                && birthday.ServerId == serverId)
                .Select(birthday => birthday.UserId)
                .ToList<UserId>();

            return users;
        }

        public abstract Task LoadUserBirthdaysAsync(); // Loads data from original source into internal storage

        public abstract Task SaveChangesAsync(); // Saves changes into the original source; Name / params subject to change
    }
}
