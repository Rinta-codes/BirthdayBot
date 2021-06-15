using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BirthdayBot.Data
{
    using UserId = String;

    public interface IBirthdaysRepository
    {
        public void AddUserBirthday(string userId, DateTime birthdayDate);

        public void DeleteUserBirthday(string userId);

        public void AdjustUserBirthday(string userId, DateTime newBirthdayDate);

        public List<UserId> LookupUsersByBirthday(DateTime birthdayDate);

        public Task LoadUserBirthdaysAsync();
    }
}
