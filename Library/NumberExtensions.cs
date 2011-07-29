using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;

namespace System
{
    /// <summary>
    /// Wetnap extension methods for various .NET numeric types.
    /// </summary>
    partial class WetnapExtensions
    {
        #region int

        /// <summary>
        /// Indicates whether this number is greater than 0.
        /// </summary>
        public static bool IsPositive(this int value)
        {
            return value > 0;
        }

        /// <summary>
        /// Indicates whether this number is less than 0.
        /// </summary>
        public static bool IsNegative(this int value)
        {
            return value < 0;
        }

        /// <summary>
        /// Returns the ToString with invariant formatting passed.  Value will be things like "123432"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToInvariant(this int value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }


        /// <summary>
        /// If the value is positive. the action is performed.  This can just save a few lines of code.
        /// </summary>
        /// <param name="actionIfPositive">The action invoked when the value is positive.</param>
        public static void IfPositive(this int val, Action actionIfPositive)
        {
            if (val > 0)
                actionIfPositive();
        }

        /// <summary>
        /// If the value is positive. the action is performed.  This can just save a few lines of code.
        /// </summary>
        /// <param name="actionIfPositive">The action invoked when the value is positive.  This includes the value as a parameter, which is useful for values that are calculated.</param>
        public static void IfPositive(this int val, Action<int> actionIfPositive)
        {
            if (val > 0)
                actionIfPositive(val);
        }

        /// <summary>
        /// Small helper to return a string that cooresponds to the correct value, to prevent things like "1 Items"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="singularText">The value to return when the value is 1.  The number value is not included.</param>
        /// <param name="pluralText">The value to be appended after the value.  The number value is included.</param>
        /// <returns></returns>
        public static string ToProperPlurality(this int value, string singularText, string pluralText, bool includeNumber)
        {
            return value.ToProperPlurality(pluralText, singularText, pluralText, includeNumber);
        }

        /// <summary>
        /// Small helper to return a string that cooresponds to the correct value, to prevent things like "1 Items"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="zeroText">The value to return if the value is 0.  The number value is not included.</param>
        /// <param name="singularText">The value to return when the value is 1.  The number value is not included.</param>
        /// <param name="pluralText">The value to be appended after the value.  The number value is included.</param>
        /// <returns></returns>
        public static string ToProperPlurality(this int value, string zeroText, string singularText, string pluralText, bool includeNumber)
        {
            switch (value)
            {
                case 0:
                    return zeroText;
                case 1:
                    return singularText;
                default:
                    if (includeNumber)
                        return value + " " + pluralText;
                    return pluralText;
            }
        }


        /// <summary>
        /// Returns integer values from (including) this value up to and including limit.   So 4.UpTo(7) would return 4,5,6,7
        /// </summary>
        /// <param name="value"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static IEnumerable<int> UpTo(this int value, int limit)
        {
            for (; value <= limit; value++)
                yield return value;
        }


        #endregion

        #region double

        /// <summary>
        /// Returns the ToString with culture-invariant formatting passed.  Value will be things like "123432.324"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToInvariant(this double value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Indicates whether this number is greater than 0.
        /// </summary>
        public static bool IsPositive(this double value)
        {
            return value > 0;
        }

        /// <summary>
        /// Indicates whether this number is less than 0.
        /// </summary>
        public static bool IsNegative(this double value)
        {
            return value < 0;
        }


        /// <summary>
        /// Converts the number to the given format. This is nice because you don't have to remember or look up format strings.  Uses the current thread's culture.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="format">The type of format to use.</param>
        /// <param name="culture">The culture info used to format.  If null, the Thread.CurrentThread.CurrentCulture will be used.</param>
        /// <param name="decimals">The number of decimals to force the formatted value to.  If null, the amount of natural decimals will be used.</param>
        public static string ToString(this double value, NumberFormat format)
        {
            return value.ToString(format, null, null);
        }

        /// <summary>
        /// Converts the number to the given format. This is nice because you don't have to remember or look up format strings.  Uses the current thread's culture.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="format">The type of format to use.</param>
        /// <param name="decimals">The number of decimals to force the formatted value to.  If null, the amount of natural decimals will be used.</param>
        public static string ToString(this double value, NumberFormat format, int? decimals)
        {

            return value.ToString(format, null, decimals);
        }

        /// <summary>
        /// Converts the number to the given format. This is nice because you don't have to remember or look up format strings.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="format">The type of format to use.</param>
        /// <param name="culture">The culture info used to format.  If null, the Thread.CurrentThread.CurrentCulture will be used.</param>
        public static string ToString(this double value, NumberFormat format, CultureInfo culture)
        {
            return value.ToString(format, culture, null);
        }

        /// <summary>
        /// Converts the number to the given format. This is nice because you don't have to remember or look up format strings.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="format">The type of format to use.</param>
        /// <param name="culture">The culture info used to format.  If null, the Thread.CurrentThread.CurrentCulture will be used.</param>
        /// <param name="decimals">The number of decimals to force the formatted value to.  If null, the amount of natural decimals will be used.</param>
        public static string ToString(this double value, NumberFormat format, CultureInfo culture, int? decimals)
        {
            if (decimals.HasValue && decimals.Value < 0)
                throw new ArgumentOutOfRangeException("decimals must be 0 or greater.");

            if (culture == null)
                culture = Threading.Thread.CurrentThread.CurrentCulture;


            string formatString;
            switch (format)
            {
                case NumberFormat.FixedPoint:
                    formatString = "f" + decimals;
                    break;
                case NumberFormat.General:
                    formatString = "g";
                    break;
                case NumberFormat.Number:
                    formatString = "n" + decimals;
                    break;
                case NumberFormat.Currency:
                    formatString = "c" + decimals;
                    break;
                case NumberFormat.Percentage:
                    formatString = "p" + decimals;
                    break;
                case NumberFormat.Scientific:
                    formatString = "e" + decimals;
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
            return value.ToString(formatString, culture.NumberFormat);
        }



        #endregion
    }


    /// <summary>
    /// Represents the display format for numeric values.
    /// </summary>
    public enum NumberFormat
    {
        /// <summary>
        /// The value is rendered with a fixed number of decimals.
        /// </summary>
        FixedPoint,
        /// <summary>
        /// A basic format that shows only the exact digits and decimals.  This ignores the Decimals property.  Does not include thousands separator.  Ex: 12.1, 22.41233, 10, -12.901
        /// </summary>
        General,
        /// <summary>
        /// The value is formatted as number, with thousand separator.
        /// </summary>
        Number,
        /// <summary>
        /// The value is formatted as a currency.
        /// </summary>
        Currency,
        /// <summary>
        /// The value is formatted as a percentage.
        /// </summary>
        Percentage,
        /// <summary>
        /// The value is formatted using scientific notation.
        /// </summary>
        Scientific,

    }

}
