using System;
using BirthdayBot;
using System.Globalization;

namespace BirthdayBot.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToBirthdayFormat(this DateTime BirthdayDate)
        {
            return BirthdayDate.ToString(BirthdayBot._birthdayDateFormat);
        }
    }

    public static class StringExtensions
    {
        public static bool FromBirthdayFormat(this string BirthdayDate, out DateTime date)
        {
            return DateTime.TryParseExact(BirthdayDate, BirthdayBot._birthdayDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
    }
}

