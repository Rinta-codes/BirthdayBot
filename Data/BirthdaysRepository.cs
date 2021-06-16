using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BirthdayBot.Data
{
    using UserId = String;

    /*
     * This abstract class implements only the data management part of the interface.
     * Various ways of loading initial data (from config, from database, etc.) 
     * are supplied by concrete classes derived from this one.
     */
    public abstract class BirthdaysRepository : IBirthdaysRepository
    {
        // Since I will need to look up Users from Birthdays, which is a
        // many-to-one relation, List of tuples makes more sense than a
        // Dictionary
        private List<(string UserId, DateTime BirthdayDate)> _birthdays = new();

        // Separately stored list of User IDs for fast duplicate check
        private HashSet<string> userIds = new();

        private bool IsUserDuplicate(string userId)
        {
            if (userIds.Contains(userId))
                return true;
            else
                return false;
        }

        protected void AddUserBirthdayInternalStorage(string userId, DateTime birthdayDate)
        {
            _birthdays.Add((userId, birthdayDate));
            userIds.Add(userId);
        }

        public void AddUserBirthday(string userId, DateTime birthdayDate)
        {
            if (IsUserDuplicate(userId))
                throw new ArgumentException($"Failed to add a new User Birthday. The following UserId already exists: {userId}");
            else
            {
                AddUserBirthdayInternalStorage(userId, birthdayDate);
                SaveChanges();
            }
        }

        public void DeleteUserBirthday(string userId) // TBU
        {
            // TBU
            SaveChanges();
        }

        public void AdjustUserBirthday(string userId, DateTime newBirthdayDate) // TBU
        {
            // TBU
            SaveChanges();
        }

        public List<UserId> LookupUsersByBirthday(DateTime birthdayDate)
        {
            List<UserId> users = new();

            foreach (var pairIdBirthday in _birthdays)
            {
                if (pairIdBirthday.BirthdayDate.Date == birthdayDate.Date)
                {
                    users.Add(pairIdBirthday.UserId);
                }
            }

            return users;
        }

        public abstract Task LoadUserBirthdaysAsync();

        // Saves changes into the original source; Name / params subject to change
        public abstract Task SaveChanges();
    }
}
