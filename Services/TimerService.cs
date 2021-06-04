using System;
using System.Collections.Generic;
using System.Timers;

namespace BirthdayBot.Services
{
    public struct Interval
    {
        public const int
        SECOND = 1000,
        MINUTE = 60000,
        HOUR = 3600000,
        DAY = 86400000,
        WEEK = 604800000;
    }

    /**
     * Customizeable timer with exposed Elapsed event
     */
    class TimerService : IDisposable
    {
        private Timer timer;

        public void SetTimer(int period = Interval.DAY)
        {
            timer = new System.Timers.Timer((double)period);
        }

        public event ElapsedEventHandler TimerEvent
        {
            add { timer.Elapsed += value; }
            remove { timer.Elapsed -= value; }
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }


    /**
     * Timer Factory
     */
    class TimerFactory : IDisposable
    {
        private List<(int period, Timer timer)> timers;
        public TimerFactory()
        {
            timers = new();
        }

        public Timer CreateTimer(int period)
        {
            Timer timer = null;

            /**
             * Check if we already have a timer activated for this period:
             * Create comparer that compares purely by period, then use it
             * to sort timers and perform a lookup.
             */
            IComparer<(int period, Timer timer)> comparer = Comparer<(int period, Timer timer)>.Create((x, y) => x.period.CompareTo(y.period));

            timers.Sort(comparer);
            int searchResult = timers.BinarySearch((period, null), comparer);

            if (searchResult > -1)
            {
                timer = timers[searchResult].timer;
            }
            else
            {
                timer = new((double)period);
            }

            timers.Add((period, timer));
            timers.Sort(comparer); // We already sort at the start, however I want to
                                   // keep timers sorted at all times as a precaution

            Console.WriteLine("Timer initialized - {0} milliseconds", timer.Interval.ToString());
            return timer;
        }

        public void Dispose()
        {
            foreach (var entry in timers)
            {
                entry.timer.Dispose();
            }
        }
    }
}
