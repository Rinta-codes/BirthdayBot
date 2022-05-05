using System;

namespace BirthdayBot.ActionAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    class TimerAttribute : Attribute
    {
        public int IntervalInMilliseconds { get; }

        public TimerAttribute(int interval)
        {
            IntervalInMilliseconds = interval;
        }
    }
}
