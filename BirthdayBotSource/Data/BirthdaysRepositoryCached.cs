using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BirthdayBot.Data
{
    using UserId = String;

    /*
     * This abstract class implements cache storage of birthday data 
     * along with part of the interface that will have to interact with cache.
     * 
     * Various ways of loading initial data (from config, from database, etc.) 
     * and saving changed data are supplied by concrete classes derived from this one.
     */
    public abstract class BirthdaysRepositoryCached : IBirthdaysRepository
    {
        // Since I will need to look up Users from Birthdays, which is a
        // many-to-one relation, List of tuples makes more sense than a
        // Dictionary
        private List<(string UserId, DateTime BirthdayDate)> _birthdaysCache = new();

        // Separately stored list of User IDs for fast duplicate check
        private HashSet<string> userIdsCache = new();

        private bool IsUserDuplicate(string userId)
        {
            if (userIdsCache.Contains(userId))
                return true;
            else
                return false;
        }

        protected void AddUserBirthdayInternalStorage(string userId, DateTime birthdayDate)
        {
            _birthdaysCache.Add((userId, birthdayDate));
            userIdsCache.Add(userId);
        }

        public async Task AddUserBirthdayAsync(string userId, DateTime birthdayDate)
        {
            if (IsUserDuplicate(userId))
                throw new ArgumentException($"Failed to add a new User Birthday. The following UserId already exists: {userId}");
            else
            {
                AddUserBirthdayInternalStorage(userId, birthdayDate);
                await SaveChangesAsync();
            }
        }

        public async Task DeleteUserBirthdayAsync(string userId) // TBU
        {
            // TBU
            await SaveChangesAsync();
        }

        public async Task AdjustUserBirthdayAsync(string userId, DateTime newBirthdayDate) // TBU
        {
            // TBU
            await SaveChangesAsync();
        }

        public async Task<List<UserId>> LookupUsersByBirthday(DateTime birthdayDate)
        {
            List<UserId> users = _birthdaysCache
                .Where(pairIdBirthday => pairIdBirthday.BirthdayDate.Date == birthdayDate.Date)
                .Select(pairIdBirthday => pairIdBirthday.UserId)
                .ToList<UserId>();


            return users;
        }

        public abstract Task LoadUserBirthdaysAsync(); // Loads data from original source into internal storage

        public abstract Task SaveChangesAsync(); // Saves changes into the original source; Name / params subject to change
    }
}
