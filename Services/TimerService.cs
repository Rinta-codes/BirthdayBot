using System;
using System.Timers;
using System.Collections.Generic;

namespace BirthdayBot.Services
{
    public struct interval
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

        public void SetTimer(int period = interval.DAY)
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
        private List<(int, Timer)> timers;
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
            IComparer<(int, Timer)> comparer = Comparer<(int, Timer)>.Create((x, y) => x.Item1.CompareTo(y.Item1));

            timers.Sort(comparer);
            int searchResult = timers.BinarySearch((period, null), comparer);

            if (searchResult > -1)
            {
                timer = timers[searchResult].Item2;
            }
            else
            {
                timer = new((double)period);
            }

            timers.Add((period, timer));
            timers.Sort(comparer); // We already sort at the start, however I want to
                                   // keep timers sorted at all times as a precaution
            return timer;
        }

        public void Dispose()
        {
            foreach (var entry in timers)
            {
                entry.Item2.Dispose();
            }
        }
    }
}
