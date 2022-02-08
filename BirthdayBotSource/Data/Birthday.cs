using System;

namespace BirthdayBot.Data
{
    /// <summary>
    /// Class for storing Birthday data
    /// </summary>
    /// <remarks>
    /// Primary Key: userId
    /// Data is stored separately per server to respect user privacy
    /// In case of serverId == null, data is applied to all servers without a server-specific setting
    /// </remarks>
    public class Birthday
    {
        public string UserId { get; }
        public DateTime BirthdayDate { get; }
        public string ServerId { get; }

        public Birthday(string userId, DateTime birthdayDate, string serverId = null)
        {
            UserId = userId;
            BirthdayDate = birthdayDate;
            ServerId = serverId;
        }
    }
}
