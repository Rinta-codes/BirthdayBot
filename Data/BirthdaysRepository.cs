using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BirthdayBot.Data
{
    using UserID = System.String;

    class BirthdaysRepository
    {
        // Since I will need to look up Users from Birthdays, which is a
        // many-to-one relation, List of tuples makes more sense than a
        // Dictionary
        private List<(string UserID, DateTime BirthdayDate)> _birthdays = new();

        // Separately stored list of User IDs for fast duplicate check
        HashSet<string> userIDs = new();

        private readonly string birthdayDateFormat = "dd MMM"; // Might change to read this from config
                                                               // or mayhaps a hardcoded singleton


        private bool IsUserDuplicate(string userID)
        {
            if (userIDs.Contains(userID))
                return true;
            else
                return false;
        }

        public void AddUserBirthday(string userID, DateTime birthdayDate)
        {
            if (IsUserDuplicate(userID))
                throw new ArgumentException($"Failed to add a new User Birthday. The following UserID already exists: {userID}");
            else
                _birthdays.Add((userID, birthdayDate));
        }

        public void DeleteUserBirthday(string userID) // TBU
        { }

        public void AdjustUserBirthday(string userID, DateTime newBirthdayDate) // TBU
        { }

        public void LoadUserBirthdaysFromConfig(IConfiguration config)
        {
            DateTime date = DateTime.Today;
            UserID id = new(String.Empty);

            foreach (var pairIdBirthday in config.GetSection("Birthdays").Get<IConfigurationSection[]>())
            {
                if (DateTime.TryParseExact(pairIdBirthday["Date"], birthdayDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    id = pairIdBirthday["Id"];

                    AddUserBirthday(id, date);
                    userIDs.Add(id);

                    Console.WriteLine($"Birthday loaded: {pairIdBirthday["Id"]} - {date}");
                }
                else
                {
                    throw new FormatException("One or more Birthday Dates in the configuration file have incorrect format; Expected format: \"" + birthdayDateFormat + "\"");
                }
            }
        }

        public void LoadUserBirthdaysFromDatabase() // TBU
        { }


        public List<UserID> LookupUsersByBirthday(DateTime birthdayDate)
        {
            List<UserID> users = new();

            foreach (var pairIdBirthday in _birthdays)
            {
                if (pairIdBirthday.BirthdayDate.Date == birthdayDate.Date)
                {
                    users.Add(pairIdBirthday.UserID);
                }
            }

            return users;
        }
    }
}
