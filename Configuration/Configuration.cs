using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Classes to hold different parts of bot config
/// </summary>
namespace BirthdayBot.Configuration
{
    public class ConnectionConfiguration
    {
        public ConnectionConfiguration()
        {
            Token = "";
        }
        public string Token { get; set; }
    }

    public class CommandsConfiguration
    {
        public CommandsConfiguration()
        {
            Prefix = "";
        }

        public string Prefix { get; set; }
    }

    public class BirthdayConfiguration
    {
        public BirthdayConfiguration()
        {
            RoleName = "Birthday Cake";
        }
        public string RoleName { get; set; }
    }
}
