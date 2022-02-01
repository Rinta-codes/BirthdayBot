using BirthdayBot.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BirthdayBotTest
{
    [TestClass]
    public class BirthdayRepositoryTest
    {
        /// <summary>
        /// Test for IBirthdayRepository.LookUpUserByBirthday()
        /// Input: Normal data; Unsorted; Expected result is not first entry
        /// </summary>
        [TestMethod]
        public async Task LookupUsersByBirthdayTest1()
        {
            Dictionary<string, string> testData = new()
            {
                [$"Birthdays:0:Id"]     = "1234567890",
                [$"Birthdays:0:Date"]   = "02 Aug",
                [$"Birthdays:1:Id"]     = "0987654321",
                [$"Birthdays:1:Date"]   = "26 Jan",
            };

            DateTime date = DateTime.Parse("26 Jan");

            List<string> expectedUsers = new()
            {
                "0987654321"
            };

            IConfiguration testConfig = new ConfigurationBuilder().AddInMemoryCollection(testData).Build();
            BirthdaysRepositoryCachedConfig birthdays = new(testConfig);
            await birthdays.LoadUserBirthdaysAsync();

            var actualUsers = await birthdays.LookupUsersByBirthday(date);

            CollectionAssert.AreEqual(expectedUsers, actualUsers);
        }

        /// <summary>
        /// Test for IBirthdayRepository.LookUpUserByBirthday()
        /// Input: Empty dataset
        /// </summary>
        [TestMethod]
        public async Task LookupUsersByBirthdayTest2()
        {
            Dictionary<string, string> testData = new() {};

            DateTime date = DateTime.Parse("26 Jan");

            List<string> expectedUsers = new() {};

            IConfiguration testConfig = new ConfigurationBuilder().AddInMemoryCollection(testData).Build();
            BirthdaysRepositoryCachedConfig birthdays = new(testConfig);
            await birthdays.LoadUserBirthdaysAsync();

            var actualUsers = await birthdays.LookupUsersByBirthday(date);

            CollectionAssert.AreEqual(expectedUsers, actualUsers);
        }
    }
}
