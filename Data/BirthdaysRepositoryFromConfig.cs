using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace BirthdayBot.Data
{
    using UserId = String;

    class BirthdaysRepositoryFromConfig : BirthdaysRepository
    {
        private readonly IConfiguration _config;

        private readonly string birthdayDateFormat = "dd MMM"; // Might change to read this from config
                                                               // or mayhaps a hardcoded singleton

        public BirthdaysRepositoryFromConfig(IConfiguration config)
        {
            _config = config;
        }


        private void LoadUserBirthdays()
        {
            DateTime date = DateTime.Today;
            UserId id = new(String.Empty);

            foreach (var pairIdBirthday in _config.GetSection("Birthdays").Get<IConfigurationSection[]>())
            {
                if (DateTime.TryParseExact(pairIdBirthday["Date"], birthdayDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    id = pairIdBirthday["Id"];

                    AddUserBirthday(id, date);

                    Console.WriteLine($"Birthday loaded: {pairIdBirthday["Id"]} - {date}");
                }
                else
                {
                    throw new FormatException("One or more Birthday Dates in the configuration file have incorrect format; Expected format: \"" + birthdayDateFormat + "\"");
                }
            }
        }

        public override async Task LoadUserBirthdaysAsync()
        {
            LoadUserBirthdays();
        }

    }
}
