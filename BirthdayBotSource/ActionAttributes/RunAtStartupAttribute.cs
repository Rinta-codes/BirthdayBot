using System;

namespace BirthdayBot.ActionAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    class RunAtStartupAttribute : Attribute
    { }
}
