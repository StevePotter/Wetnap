using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.ComponentModel;

namespace System
{
    /// <summary>
    /// Wetnap extension methods for DateTime.
    /// </summary>
    partial class WetnapExtensions
    {

        /// <summary>
        /// Calculates the time between this date and 1/1/1970, also known as Unix or POSIX time.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static TimeSpan SinceEpoch(this DateTime date)
        {
            return (date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime()) - Epoch;
        }
        static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        /// <summary>
        /// Indicates whether the current date falls on the same exact day as the other date.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="otherday"></param>
        /// <returns></returns>
        public static bool IsSameDay(this DateTime date, DateTime other)
        {
            return date.Date == other.Date;
        }

        /// <summary>
        /// Converts the date to the given format, using the current thread's CurrentCulture value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormattedString(this DateTime value, DateTimeFormat format)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var formatter = culture.DateTimeFormat;
            string formatString = null;
            switch (format)
            {
                case DateTimeFormat.ShortDate:
                    formatString = formatter.ShortDatePattern;
                    break;
                case DateTimeFormat.ShortTime:
                    formatString = formatter.ShortTimePattern;
                    break;
                case DateTimeFormat.LongDate:
                    formatString = formatter.LongDatePattern;
                    break;
                case DateTimeFormat.LongTime:
                    formatString = formatter.LongTimePattern;
                    break;
                case DateTimeFormat.ShortDateTime:
                    formatString = formatter.ShortDatePattern + " " + formatter.ShortTimePattern;//not gonna be correct for all cultures
                    break;
                case DateTimeFormat.LongDateTime:
                    formatString = formatter.FullDateTimePattern;
                    break;
                case DateTimeFormat.MonthDay:
                    formatString = formatter.MonthDayPattern;
                    break;
                case DateTimeFormat.YearMonth:
                    formatString = formatter.YearMonthPattern;
                    break;
            }
            return formatString == null ? value.ToString(formatter) : value.ToString(formatString, formatter);
        }

    }


    /// <summary>
    /// Specifies common DateTime format, which is easier than entering the actual pattern.
    /// </summary>
    public enum DateTimeFormat
    {
        /// <summary>
        /// No format set, meaning the default culture setting will be used.
        /// </summary>
        NotSet,
        /// <summary>
        /// Represents the short date specified in System.Globalization.DateTimeFormatInfo.ShortDatePattern;
        /// </summary>
        ShortDate,
        /// <summary>
        /// Represents the short time as specified in System.Globalization.DateTimeFormatInfo.ShortTime.
        /// </summary>
        ShortTime,
        /// <summary>
        /// Represents the long date as specified in System.Globalization.DateTimeFormatInfo.LongDatePattern.
        /// </summary>
        LongDate,
        /// <summary>
        /// Represents the long time as specified in System.Globalization.DateTimeFormatInfo.LongTimePattern.
        /// </summary>
        LongTime,
        /// <summary>
        /// Represents the short date and time.  NOTE: System.Globalization.DateTimeFormatInfo 
        /// does not have a culture-specific property for this.  The pattern is taken by concatenating the ShortDatePattern 
        /// and ShortTimePattern with a space in the middle.  Therefore it is not guaranteed be entirely culturally correct, although most cultures put time after date.
        /// </summary>
        ShortDateTime,
        /// <summary>
        /// Represents the long date and time as specified in System.Globalization.DateTimeFormatInfo.LongDatePattern.
        /// </summary>
        LongDateTime,
        /// <summary>
        /// Represents the month and day as specified in System.Globalization.DateTimeFormatInfo.MonthDayPattern.  Normally the month is spelled out.
        /// </summary>
        MonthDay,
        /// <summary>
        /// Represents the year and month as specified in System.Globalization.DateTimeFormatInfo.YearMonthPattern.  Normally the month is spelled out.
        /// </summary>
        YearMonth
    }

}
