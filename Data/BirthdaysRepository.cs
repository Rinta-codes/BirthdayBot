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
        private HashSet<string> userIDs = new();

        private bool IsUserDuplicate(string userId)
        {
            if (userIDs.Contains(userId))
                return true;
            else
                return false;
        }

        public void AddUserBirthday(string userId, DateTime birthdayDate)
        {
            if (IsUserDuplicate(userId))
                throw new ArgumentException($"Failed to add a new User Birthday. The following UserId already exists: {userId}");
            else
            {
                _birthdays.Add((userId, birthdayDate));
                userIDs.Add(userId);
            }
        }

        public void DeleteUserBirthday(string userId) // TBU
        { }

        public void AdjustUserBirthday(string userId, DateTime newBirthdayDate) // TBU
        { }

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
    }
}
