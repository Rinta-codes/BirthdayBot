using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Threading.Tasks;
using BirthdayBot.Extensions;

namespace BirthdayBot.Data
{
    using UserId = String;

    /*
     * This class details data retrieval and data upload process for BirthdayRepositoryCached
     * when birthday data is stored in config file
     * 
     * * Will require minor changes when config is no longer part of DI container
     */
    public class BirthdaysRepositoryCachedConfig : BirthdaysRepositoryCached
    {
        private readonly IConfiguration _config;

        public BirthdaysRepositoryCachedConfig(IConfiguration config)
        {
            _config = config;
        }


        private void LoadUserBirthdays()
        {
            DateTime date = DateTime.Today;
            UserId id = new(String.Empty);

            foreach (var pairIdBirthday in _config.GetSection("Birthdays").Get<IConfigurationSection[]>())
            {
                if (pairIdBirthday["Date"].FromBirthdayFormat(out date))
                {
                    id = pairIdBirthday["Id"];

                    AddUserBirthdayInternalStorage(id, date);

                    Console.WriteLine($"Birthday loaded: {pairIdBirthday["Id"]} - {date}");
                }
                else
                {
                    throw new FormatException("Could not parse one or more birthday dates from configuration. Did you use correct date format?");
                }
            }
        }

        public override async Task LoadUserBirthdaysAsync()
        {
            LoadUserBirthdays();
        }

        public override async Task SaveChangesAsync() // TBU
        {
            // TBU
        }

    }
}
