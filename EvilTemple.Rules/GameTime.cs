#region

using System.Diagnostics;

#endregion

namespace EvilTemple.Rules
{
    public delegate void TimeChangeHandler(GameTime oldTime, GameTime newTime);

    public delegate void HourChangeHandler(int oldHour, GameTime newTime);

    /// <summary>
    ///   The current game time.
    /// </summary>
    public class GameTime
    {
        public const int SecondsPerMinute = 60;
        public const int MinutesPerHour = 60;
        public const int SecondsPerHour = MinutesPerHour*SecondsPerMinute;
        public const int HoursPerDay = 24;
        public const int SecondsPerDay = HoursPerDay*SecondsPerHour;
        public const int DaysPerMonth = 28;
        public const int SecondsPerMonth = DaysPerMonth*SecondsPerDay;
        public const int SecondsPerYear = 12*SecondsPerMonth;

        public event TimeChangeHandler OnTimeChange;

        public event HourChangeHandler OnHourChange;

        public GameTime()
        {
        }

        public GameTime(int year, int month = 1, int day = 1, int hour = 0, int minute = 0, int second = 0)
        {
            SetTo(year, month, day, hour, minute, second);
        }

        private void NormalizeTime()
        {
            var wrappedYears = SecondOfTheYear/SecondsPerYear;
            SecondOfTheYear %= SecondsPerYear;

            if (wrappedYears > 0)
            {
                Year += wrappedYears;
                Trace.TraceInformation("Time overflow. Adding " + wrappedYears + " years.");
            }
        }

        /// <summary>
        ///   Returns a time reference representing the current time.
        /// </summary>
        /// <returns>A reference to the current time.</returns>
        public GameTime Copy()
        {
            NormalizeTime();
            return new GameTime
                       {
                           SecondOfTheYear = SecondOfTheYear,
                           Year = Year
                       };
        }

        /// <summary>
        ///   Sets the game time to a specific point in time.
        /// </summary>
        /// <param name = "year">The year.</param>
        /// <param name = "month">The month of the year. (1-11)  (Default: 1)</param>
        /// <param name = "day">The day of the month. (1-28) (Default: 1)</param>
        /// <param name = "hour">The hour of the day. (0-23) (Default: 0)</param>
        /// <param name = "minute">The minute of the hour (0-59) (Default: 0)</param>
        /// <param name = "second">The second of the hour (0-59) (Default: 0)</param>
        public void SetTo(int year, int month = 1, int day = 1, int hour = 0, int minute = 0, int second = 0)
        {
            var oldTime = Copy();

            month -= 1;
            day -= 1;

            SecondOfTheYear = month*SecondsPerMonth
                              + day*SecondsPerDay
                              + hour*SecondsPerHour
                              + minute*SecondsPerMinute
                              + second;

            Year = year;
            NormalizeTime();

            InvokeOnTimeChange(oldTime);

            if (oldTime.HourOfDay != HourOfDay)
                InvokeOnHourChange(oldTime.HourOfDay);
        }

        /// <summary>
        /// Sets this game time to the time represented by another game time object.
        /// </summary>
        /// <param name="other"></param>
        public void SetTo(GameTime other)
        {
            var oldTime = Copy();

            Year = other.Year;
            SecondOfTheYear = other.SecondOfTheYear;
            NormalizeTime();

            InvokeOnTimeChange(oldTime);
            
            if (oldTime.HourOfDay != HourOfDay)
                InvokeOnHourChange(oldTime.HourOfDay);
        }

        /// <summary>
        ///   Progress game time.
        /// </summary>
        /// <param name = "years">The number of years to add.</param>
        /// <param name = "months">The number of months to add.</param>
        /// <param name = "days">The number of days to add.</param>
        /// <param name = "hours">The number of hours to add.</param>
        /// <param name = "minutes">The number of minutes to add.</param>
        /// <param name = "seconds">The number of seconds to progress game time by.</param>
        public void Advance(int years = 0, int months = 0, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
        {
            seconds += minutes*SecondsPerMinute;
            seconds += hours*SecondsPerHour;
            seconds += days*SecondsPerDay;
            seconds += months*SecondsPerMonth;
            seconds += years*SecondsPerYear;
            
            var oldTime = Copy();

            SecondOfTheYear += seconds;
            NormalizeTime();

            InvokeOnTimeChange(oldTime);

            if (oldTime.HourOfDay != HourOfDay)
                InvokeOnHourChange(oldTime.HourOfDay);
        }

        public int SecondOfMinute
        {
            get { return SecondOfTheYear%SecondsPerMinute; }
        }

        public int MinuteOfHour
        {
            get { return (SecondOfTheYear/SecondsPerMinute)%MinutesPerHour; }
        }

        public int HourOfDay
        {
            get { return (SecondOfTheYear/SecondsPerHour)%HoursPerDay; }
        }

        public int DayOfMonth
        {
            get { return 1 + (SecondOfTheYear/SecondsPerDay)%DaysPerMonth; }
        }

        public int Month
        {
            get { return 1 + (SecondOfTheYear/SecondsPerMonth); }
        }

        public int Year { get; private set; }

        public int SecondOfTheYear { get; private set; }

        public bool IsDaytime
        {
            get
            {
                var hour = HourOfDay;
                return hour >= 6 && hour < 18;
            }
        }

        public bool IsNighttime
        {
            get { return !IsDaytime; }
        }

        private void InvokeOnTimeChange(GameTime oldTime)
        {
            var handler = OnTimeChange;
            if (handler != null) handler(oldTime, this);
        }

        private void InvokeOnHourChange(int oldHour)
        {
            var handler = OnHourChange;
            if (handler != null) handler(oldHour, this);
        }

        /*StartupListeners.add(function() {
        SaveGames.addLoadingListener(load);
        SaveGames.addSavingListener(save);

        var gameTimeTick() {
            void addTime(1); // One minute per second for now.
            gameView.addVisualTimer(100, gameTimeTick);
        };
        gameView.addVisualTimer(1000, gameTimeTick);


    })*/
    }
}