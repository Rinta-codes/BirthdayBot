using BirthdayBot.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace BirthdayBotTest
{
    [TestClass]
    public class BirthdaysRepositoryTest
    {
        public Stream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize<BirthdaysArray>(new BirthdaysArray(new List<Birthday>()))));
            // new MemoryStream(System.Text.Encoding.UTF8.GetBytes("{\"Birthdays\":[]}"));

        /// <summary>
        /// Test for BirthdaysRepositoryCachedConfig.LookUpUserByBirthday()
        /// Input: Normal data; Unsorted; Single result expected; Expected result is not first entry
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
            BirthdaysRepositoryFromJson<BirthdaysCacheMemory> birthdays = new(testConfig, stream);
            await birthdays.LoadFromSourceAsync();

            var actualUsers = await birthdays.LookupUsersByBirthdayAsync(date);

            CollectionAssert.AreEqual(expectedUsers, actualUsers);
        }

        /// <summary>
        /// Test for BirthdaysRepositoryCachedConfig.LookUpUserByBirthday()
        /// Input: Normal data; Unsorted; Multiple results expected
        /// </summary>
        [TestMethod]
        public async Task LookupUsersByBirthdayTest2()
        {
            Dictionary<string, string> testData = new()
            {
                [$"Birthdays:0:Id"] = "12345",
                [$"Birthdays:0:Date"] = "02 Aug",
                [$"Birthdays:1:Id"] = "23456",
                [$"Birthdays:1:Date"] = "01 Aug", // Same month, different date
                [$"Birthdays:2:Id"] = "45678",
                [$"Birthdays:2:Date"] = "02 Aug",
                [$"Birthdays:3:Id"] = "56789",
                [$"Birthdays:3:Date"] = "02 Jan", // Same date, different month
                [$"Birthdays:4:Id"] = "67890",
                [$"Birthdays:4:Date"] = "02 Aug",
                [$"Birthdays:5:Id"] = "78901",
                [$"Birthdays:5:Date"] = "08 Feb", // An odd "08/02" in a crowd of "02/08"s
            };

            DateTime date = DateTime.Parse("02 Aug");

            List<string> expectedUsers = new()
            {
                "12345",
                "45678",
                "67890"
            };

            IConfiguration testConfig = new ConfigurationBuilder().AddInMemoryCollection(testData).Build();
            BirthdaysRepositoryFromJson<BirthdaysCacheMemory> birthdays = new(testConfig, stream);
            await birthdays.LoadFromSourceAsync();

            var actualUsers = await birthdays.LookupUsersByBirthdayAsync(date);

            CollectionAssert.AreEqual(expectedUsers, actualUsers);
        }

        /// <summary>
        /// Test for BirthdaysRepositoryCachedConfig.LookUpUserByBirthday()
        /// Input: Empty dataset; No exception expected
        /// </summary>
        [TestMethod]
        public async Task LookupUsersByBirthdayTest3()
        {
            Dictionary<string, string> testData = new() {};

            DateTime date = DateTime.Parse("26 Jan");

            List<string> expectedUsers = new() {};

            IConfiguration testConfig = new ConfigurationBuilder().AddInMemoryCollection(testData).Build();
            BirthdaysRepositoryFromJson<BirthdaysCacheMemory> birthdays = new(testConfig, stream);
            await birthdays.LoadFromSourceAsync();

            var actualUsers = await birthdays.LookupUsersByBirthdayAsync(date);

            CollectionAssert.AreEqual(expectedUsers, actualUsers);
        }

        /// <summary>
        /// Test for BirthdaysRepositoryCachedConfig.LookUpUserByBirthday()
        /// Input: Normal data; Unsorted; Looking up a date that's not in the dataset; No exception expected
        /// </summary>
        [TestMethod]
        public async Task LookupUsersByBirthdayTest4()
        {
            Dictionary<string, string> testData = new()
            {
                [$"Birthdays:0:Id"] = "1234567890",
                [$"Birthdays:0:Date"] = "05 Apr",
                [$"Birthdays:1:Id"] = "0987654321",
                [$"Birthdays:1:Date"] = "17 Dec",
            };

            DateTime date = DateTime.Parse("26 Jan");

            List<string> expectedUsers = new() {};

            IConfiguration testConfig = new ConfigurationBuilder().AddInMemoryCollection(testData).Build();
            BirthdaysRepositoryFromJson<BirthdaysCacheMemory> birthdays = new(testConfig, stream);
            await birthdays.LoadFromSourceAsync();

            var actualUsers = await birthdays.LookupUsersByBirthdayAsync(date);

            CollectionAssert.AreEqual(expectedUsers, actualUsers);
        }
    }
}
