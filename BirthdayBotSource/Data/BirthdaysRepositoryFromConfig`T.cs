using BirthdayBot.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BirthdayBot.Data
{
    using UserId = String;

    /// <summary>
    /// Implements BirthdaysRepository loaded from configuration file
    /// with optional caching support.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BirthdaysRepositoryFromConfig<T> : IBirthdaysRepositoryCached where T : class, IBirthdaysCache, new()
    {
        private T _birthdaysCache;
        private readonly IConfiguration _config;

        public BirthdaysRepositoryFromConfig(IConfiguration config)
        {
            _config = config;
            _birthdaysCache = new();
        }

        public async Task AddUserBirthdayAsync(Birthday birthday)
        {
            throw new NotImplementedException();
        }

        public async Task AdjustUserBirthdayAsync(Birthday birthday)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteUserBirthdayAsync(Birthday birthday)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> LookupUsersByBirthdayAsync(DateTime birthdayDate, string serverId = null)
        {
            return await _birthdaysCache.LookupUsersByBirthdayAsync(birthdayDate, serverId);
        }

        public async Task LoadFromSourceAsync()
        {
            DateTime date = DateTime.Today;
            UserId id = new(String.Empty);

            var birthdaysRawData = _config.GetSection("Birthdays").Get<IConfigurationSection[]>();

            if (birthdaysRawData == null)
                return;

            foreach (var pairIdBirthday in birthdaysRawData)
            {
                if (pairIdBirthday["Date"].FromBirthdayFormat(out date))
                {
                    id = pairIdBirthday["Id"];

                    await _birthdaysCache.AddUserBirthdayAsync(new Birthday(id, date));

                    Console.WriteLine($"Birthday loaded: {pairIdBirthday["Id"]} - {date}");
                }
                else
                {
                    throw new FormatException("Could not parse one or more birthday dates from configuration. Did you use correct date format?");
                }
            }
        }


        public async Task SaveToSourceAsync()
        {
            throw new NotImplementedException();
        }
    }
}
