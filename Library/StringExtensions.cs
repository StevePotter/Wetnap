using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// Wetnap extension methods for strings.
    /// </summary>
    partial class WetnapExtensions
    {
        #region Checking

        private static readonly Regex alphaNumeric = new Regex("[a-zA-Z0-9]");
        private static readonly Regex alpha = new Regex("[a-zA-Z]");
        private static readonly Regex numericNoDecimal = new Regex("[0-9]");
        private static readonly Regex numericDecimal = new Regex(@"[0-9\.]");

        /// <summary>
        /// Indicates whether the value is not null and has at least one character.  It's the exact same thing as writing the clunky !string.IsNullOrEmpty(value).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasChars(this string value)
        {
            return value.HasChars(CharsThatMatter.Any);
        }

        /// <summary>
        /// Indicates whether the value is not null and has at least one character, with an option to disregard whitespace.
        /// </summary>
        /// <param name="countWhitespaceAsChars">When true, the method will return true even if there are only whitespace characters.  If false and the string is only whitespace, the method will return false.</param>
        [Obsolete("Use HasChars(CharsToCount) instead.")]
        public static bool HasChars(this string value, bool countWhitespaceAsChars)
        {
            return HasChars(value, countWhitespaceAsChars ? CharsThatMatter.Any : CharsThatMatter.NonWhitespace);
        }

        /// <summary>
        /// Indicates whether the string has at least one character that satisfies the given condition.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="charMattersCondition"></param>
        /// <returns></returns>
        public static bool HasChars(this string value, CharsThatMatter charMattersCondition)
        {
            switch (charMattersCondition)
            {
                case CharsThatMatter.Any:
                    return !string.IsNullOrEmpty(value);
                case CharsThatMatter.NonWhitespace:
                    return HasChars(value, c => !char.IsWhiteSpace(c));
                case CharsThatMatter.Letters:
                    return HasChars(value, char.IsLetter);
                case CharsThatMatter.Digits:
                    return HasChars(value, char.IsDigit);
                case CharsThatMatter.LettersAndDigits:
                    return HasChars(value, char.IsLetterOrDigit);
                default:
                    throw new InvalidEnumArgumentException("whatCounts");
            }
        }

        /// <summary>
        /// Indicates whether the string has at least one character that satisfies the given condition.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="charMattersCondition"></param>
        /// <returns></returns>
        public static bool HasChars(this string value, Predicate<char> charMattersCondition)
        {
            //if there are no characters at all we can safely return false
            if (string.IsNullOrEmpty(value))
                return false;
            return value.Any(t => charMattersCondition(t));
        }


        /// <summary>
        /// Indicates whether the string has at least one letter.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("Use value.HasChars(Char.IsLetter) instead.")]
        public static bool HasLetters(this string value)
        {
            return value.HasChars(Char.IsLetter);
        }

        /// <summary>
        /// Just a shortcut for the awkward string.IsNullOrEmpty static method.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasNoChars(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Whether the value has at least one non-whitespace one character.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("Use \"blah\".HasChars(CharsThatMatter.NonWhitespace) instead")]
        public static bool HasNonWhitespace(this string value)
        {
            return HasChars(value, CharsThatMatter.NonWhitespace);
        }

        /// <summary>
        /// Just a shortcut for Equals(StringComparison.Ordinal).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EqualsCaseInsensitive(this string value, string match)
        {
            if (value == null)
                return match == null;
            if (match == null)
                return false;
            return value.Equals(match, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Indicates whether the string passed in is equal to any of the possible matches provided.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EqualsAny(this string value, params string[] match)
        {
            return EqualsAny(value, StringComparison.Ordinal, match);
        }

        /// <summary>
        /// Indicates whether the string passed in is equal to any of the possible matches provided.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toRemove"></param>
        /// <returns></returns>
        public static bool EqualsAny(this string value, StringComparison comparison, params string[] match)
        {
            foreach (string m in match)
            {
                if (value.Equals(m, comparison))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Indicates whether the string contains the string provided.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool Contains(this string value, string match, StringComparison comparison)
        {
            return ContainsAny(value, comparison, match);
        }

        /// <summary>
        /// Indicates whether the string contains any of the strings provided.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string value, StringComparison comparison, params string[] matches)
        {
            if (matches == null || matches.Length == 0)
                throw new ArgumentException("values is empty");

            if (string.IsNullOrEmpty(value))
                return false;

            foreach (string match in matches)
            {
                int index = value.IndexOf(match, 0, comparison);
                if (index >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Removes all occurances of the given string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toRemove"></param>
        /// <returns></returns>
        public static string Strip(this string value, string toRemove)
        {
            if (!value.HasChars() || !toRemove.HasChars())
                return value;
            return value.Replace(toRemove, String.Empty);
        }

        /// <summary>
        /// Removes everything that is not in the filter text from the input.
        /// </summary>
        /// <param name="input">Input text</param>
        /// <param name="regexToKeep">Regex expression of text to keep</param>
        /// <returns>This string minus everything not in the filter text.</returns>
        /// <remarks>Thanks to http://cul.codeplex.com</remarks>
        public static string Filter(this string input, string regexToKeep)
        {
            if (input.HasNoChars())
                return input;
            if (regexToKeep.HasNoChars())
                return string.Empty;

            return Filter(input, new Regex(regexToKeep));
        }

        /// <summary>
        /// Removes everything that is not in the filter text from the input.
        /// </summary>
        /// <param name="input">Input text</param>
        /// <param name="regexToKeep">Regex expression of text to keep</param>
        /// <returns>This string minus everything not in the filter text.</returns>
        /// <remarks>Thanks to http://cul.codeplex.com</remarks>
        public static string Filter(this string input, Regex regexToKeep)
        {
            if (input.HasNoChars())
                return input;

            return string.Join(string.Empty,
                               regexToKeep.Matches(input).Cast<Match>().Select(match => match.Value).ToArray());
        }


        /// <summary>
        /// Keeps only alphanumeric characters (a-z, A-Z, 0-9)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FilterAlphaNumeric(this string input)
        {
            return input.Filter(alphaNumeric);
        }

        /// <summary>
        /// Keeps only alphanumeric characters (a-z, A-Z)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FilterAlpha(this string input)
        {
            return input.Filter(alpha);
        }


        /// <summary>
        /// Removes every character from the string that is not a number.  so "4ms" will return "4" and "4 plus 1" returns "41".
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("Use FilterNumbers(false) instead")]
        public static string FilterDigits(this string value)
        {
            if (value == null)
                return null;
            return new string(value.ToCharArray().Where(o => Char.IsDigit(o)).ToArray());
        }

        /// <summary>
        /// Keeps only numeric characters, optionally including decimal points.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <remarks>If there are multiple decimal points, they will be kept.</remarks>
        public static string FilterNumbers(this string input, bool keepDecimal)
        {
            return input.Filter(keepDecimal ? numericDecimal : numericNoDecimal);
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Converts this value to an integer, throwing an exception if there are any problems.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>Converting strings to integer happens so often that this function was deemed useful.</remarks>
        public static int ToInt(this string value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Converts this value to an integer, reiturning null f there are any problems.
        /// </summary>
        /// <remarks>Converting strings to integer happens so often that this function was deemed useful.</remarks>
        public static int? ToIntTry(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return new int?();
            int val;
            return int.TryParse(value, out val) ? val : new int?();
        }


        /// <summary>
        /// Splits the string into its given lines.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="doubleQuotes"></param>
        /// <returns></returns>
        public static string[] SplitLines(this string value)
        {
            return value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None); //will handle multiple types of line breaks.  found at http://stackoverflow.com/questions/1547476/easiest-way-to-split-a-string-on-newlines-in-net
        }

        #endregion

        #region Formatting

        /// <summary>
        /// Lowercases the first character, if one exists.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string LowerFirstChar(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            char firstChar = char.ToLower(value[0]);
            return value.Length == 1 ? firstChar.ToString() : firstChar + value.Substring(1);
        }

        /// <summary>
        /// Uppercases the first character, if one exists.  Otherwise just returns null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UpperFirstChar(this string value)
        {
            if (value == null)
                return null;
            return string.IsNullOrEmpty(value) ? value : char.ToUpper(value[0]) + value.Substring(1);
        }

        /// <summary>
        /// Adds a certain number of spaces to each line in the string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="doubleQuotes"></param>
        /// <returns></returns>
        public static string Indent(this string value, int spaces = 4)
        {
            string indent = "".PadLeft(spaces, ' ');
            string[] lines = value.SplitLines();
            return string.Join(Environment.NewLine, lines.Select(line => indent + line).ToArray());
        }


        /// <summary>
        /// If the value has characters, it returns the string.  If null or empty, it returns null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CharsOrNull(this string value)
        {
            return value.CharsOr((string)null);
        }

        /// <summary>
        /// If the value has characters, it returns the string.  If it is null or empty, it returns empty.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CharsOrEmpty(this string value)
        {
            return value.CharsOr(string.Empty);
        }

        /// <summary>
        /// If the value has characters, it returns the original string.  If it is null or empty, it returns the "returnIfNoChars" value.
        /// </summary>
        public static string CharsOr(this string value, string returnIfNoChars)
        {
            return value.HasChars() ? value : returnIfNoChars;
        }

        /// <summary>
        /// If the value has characters, it returns the original string.  If it is null or empty, it returns the "returnIfNoChars" return value.
        /// </summary>
        public static string CharsOr(this string value, Func<string> returnIfNoChars)
        {
            return value.HasChars() ? value : returnIfNoChars();
        }

        public static string CharsOr(this string value, CharsThatMatter charMattersCondition, string returnIfNoChars)
        {
            return value.HasChars(charMattersCondition) ? value : returnIfNoChars;
        }

        public static string CharsOr(this string value, CharsThatMatter charMattersCondition,
                                     Func<string> returnIfNoChars)
        {
            return value.HasChars(charMattersCondition) ? value : returnIfNoChars();
        }


        public static string CharsOr(this string value, Predicate<char> charMattersCondition, string returnIfNoChars)
        {
            return value.HasChars(charMattersCondition) ? value : returnIfNoChars;
        }

        /// <summary>
        /// If the string has any characters that match the given condition, it returns the original string.  Otherwise, it invokes the provided callback to return a "default" value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="charMattersCondition"></param>
        /// <returns></returns>
        public static string CharsOr(this string value, Predicate<char> charMattersCondition,
                                     Func<string> returnIfNoChars)
        {
            return value.HasChars(charMattersCondition) ? value : returnIfNoChars();
        }


        /// <summary>
        /// If the length of this string exceeds the max, it will be trimmed according to the type specified.
        /// </summary>
        public static string Truncate(this string value, int maxLength, StringTruncating trimMode)
        {
            return Truncate(value, maxLength, trimMode, "...");
        }

        /// <summary>
        /// If the length of this string exceeds the max, it will be trimmed according to the type specified.
        /// </summary>
        /// <param name="value">The value to be truncated.</param>
        /// <param name="maxLength">The max length of the string.</param>
        /// <param name="trimming">The type of truncation to be used.</param>
        /// <param name="ellipsis">If an ellipsis is used, this is the one used.  Default is "..."</param>
        /// <returns></returns>
        public static string Truncate(this string value, int maxLength, StringTruncating trimming, string ellipsis)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            //trim
            if (maxLength > 0 && value.Length > maxLength)
            {
                if (ellipsis == null)
                    ellipsis = "...";
                switch (trimming)
                {
                    case StringTruncating.Character:
                        value = value.Substring(0, maxLength);
                        break;
                    case StringTruncating.EllipsisCharacter:
                        value = value.Substring(0, maxLength) + ellipsis;

                        break;
                    case StringTruncating.EllipsisCenter:
                        //get the substrings and put the ellipsis between them
                        int endLength = maxLength / 2;
                        int beginningLength = maxLength - endLength;
                        //number to take from beginnning.  this is not trimlength/2 because trimlength may be odd

                        value = value.Substring(0, beginningLength) + ellipsis +
                                value.Substring(value.Length - endLength);
                        break;
                    case StringTruncating.EllipsisWord:
                        int nearestWordIndex = value.FindWordBreak(maxLength);
                        //no nearest word, so just leave the value
                        if (nearestWordIndex >= 0) value = value.Substring(0, nearestWordIndex) + ellipsis;
                        break;
                    case StringTruncating.Word:
                        nearestWordIndex = value.FindWordBreak(maxLength);
                        //no nearest word, so just leave the value
                        if (nearestWordIndex >= 0) value = value.Substring(0, nearestWordIndex);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Invalid TrimMode value.");
                }
            }

            return value;
        }




        #endregion

        #region Splitting

        #region Before, BeforeLast, After, AfterLast

        /// <summary>
        /// Default includeMatch for Before and BeforeLast.  In the future this could be exposed so devs could change defaults.
        /// </summary>
        private static bool DefaultIncludeMatchForBefore = false;
        /// <summary>
        /// Default comaprison for Before and BeforeLast.  In the future this could be exposed so devs could change defaults.
        /// </summary>
        private static StringComparison DefaultComparisonForBefore = StringComparison.Ordinal;

        /// <summary>
        /// Gets the substring that comes before the first occurance of the match is found.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <example>"I eat hot dog".Before("hot") == "I eat ".</example>
        public static string Before(this string source, string match)
        {
            return source.Before(match, DefaultComparisonForBefore);
        }

        /// <summary>
        /// Gets the substring that comes before the first occurance of the match is found, optionally appending the match to the result.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <param name="includeMatch">When true, the match is appended to the result, if the match was found.</param>
        /// <example>"I eat hot dog".Before("hot", false) == "I eat ".</example>
        /// <example>"I eat hot dog".Before("hot", true) == "I eat ".</example>
        public static string Before(this string source, string match, bool includeMatch)
        {
            return Before(source, match, 1, includeMatch, DefaultComparisonForBefore);
        }

        /// <summary>
        /// Gets the substring that comes before the first occurance of the match is found.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <param name="comparison">The type of comparison used when searching.</param>
        public static string Before(this string source, string match, StringComparison comparison)
        {
            return Before(source, match, 1, comparison);
        }

        /// <summary>
        /// Gets the substring that comes before the first occurance of the match is found, optionally appending the match to the result.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <param name="includeMatch">When true, the match is appended to the result, if the match was found.</param>
        /// <param name="comparison">The type of comparison used when searching.</param>
        public static string Before(this string source, string match, bool includeMatch, StringComparison comparison)
        {
            return Before(source, match, 1, includeMatch, comparison);
        }

        /// <summary>
        /// Gets the substring that comes before the Xth occurance of the match is found.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <param name="matchOccurance">Indicates after which occurance of the match string will be returned.  0 or less indicates the last occurance.  1 is the first, 2 is the second, etc.</param>
        /// <param name="comparison">The type of comparison used when searching.</param>
        public static string Before(this string source, string match, int matchOccurance, StringComparison comparison)
        {
            return Before(source, match, matchOccurance, DefaultIncludeMatchForBefore, comparison);
        }

        /// <summary>
        /// Gets the substring that comes before the Xth occurance of the match is found, optionally appending the match to the result.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <param name="matchOccurance">Indicates after which occurance of the match string will be returned.  0 or less indicates the last occurance.  1 is the first, 2 is the second, etc.</param>
        /// <param name="includeMatch">When true, the match is appended to the result, if the match was found.</param>
        public static string Before(this string source, string match, int matchOccurance, bool includeMatch)
        {
            return Before(source, match, matchOccurance, includeMatch, DefaultComparisonForBefore);                    
        }

        /// <summary>
        /// Gets the substring that comes before the Xth occurance of the match is found, optionally appending the match to the result.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <param name="matchOccurance">Indicates after which occurance of the match string will be returned.  0 or less indicates the last occurance.  1 is the first, 2 is the second, etc.</param>
        /// <param name="includeMatch">When true, the match is appended to the result, if the match was found.</param>
        /// <param name="comparison">The type of comparison used when searching.</param>
        public static string Before(this string source, string match, int matchOccurance, bool includeMatch, StringComparison comparison)
        {
            if (!source.HasChars() || !match.HasChars())
                return source;

            var index = source.IndexOfOccurance(match, matchOccurance, comparison);
            //index is the start of the match
            return index < 0 ? null : (includeMatch ? source.Substring(0, index + match.Length) : source.Substring(0, index));
        }

        /// <summary>
        /// Gets the substring that comes before the last occurance of the match is found.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <example>"I eat hot hot dog".Before("hot") == "I eat hot ".</example>
        public static string BeforeLast(this string value, string match)
        {
            return value.BeforeLast(match, DefaultComparisonForBefore);
        }

        /// <summary>
        /// Gets the substring that comes before the last occurance of the match is found, optionally appending the match to the result.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <param name="includeMatch">When true, the match is appended to the result, if the match was found.</param>
        public static string BeforeLast(this string source, string match, bool includeMatch)
        {
            return BeforeLast(source, match, includeMatch, DefaultComparisonForBefore);
        }

        /// <summary>
        /// Gets the substring that comes before the last occurance of the match is found.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <param name="comparison">The type of comparison used when searching.</param>
        public static string BeforeLast(this string source, string match, StringComparison comparison)
        {
            return BeforeLast(source, match, DefaultIncludeMatchForBefore, comparison);
        }

        /// <summary>
        /// Gets the substring that comes before the last occurance of the match is found, optionally appending the match to the result.  Otherwise, it returns null.
        /// </summary>
        /// <param name="source">The main string being evaluated.</param>
        /// <param name="match">The string that is being searched for within source.</param>
        /// <param name="includeMatch">When true, the match is appended to the result, if the match was found.</param>
        /// <param name="comparison">The type of comparison used when searching.</param>
        public static string BeforeLast(this string source, string match, bool includeMatch, StringComparison comparison)
        {
            return Before(source, match, 0, includeMatch, comparison);
        }


        /// <summary>
        /// Default includeMatch for After and AfterLast.  In the future this could be exposed so devs could change defaults.
        /// </summary>
        private static bool DefaultIncludeMatchForAfter = false;
        /// <summary>
        /// Default comaprison for After and AfterLast.  In the future this could be exposed so devs could change defaults.
        /// </summary>
        private static StringComparison DefaultComparisonForAfter = StringComparison.Ordinal;


        /// <summary>
        /// Gets the substring that comes after the first occurance of the match, or null if the match isn't found.
        /// </summary>
        public static string After(this string value, string match)
        {
            return value.After(match, DefaultComparisonForAfter);
        }

        /// <summary>
        /// Gets the substring that comes after the first occurance of the match, or null if the match isn't found.
        /// </summary>
        public static string After(this string value, string match, bool includeMatch)
        {
            return value.After(match, includeMatch, DefaultComparisonForAfter);
        }

        /// <summary>
        /// Gets the substring that comes after the first occurance of the match, or null if the match isn't found.
        /// </summary>
        public static string After(this string value, string match, StringComparison comparison)
        {
            return After(value, match, 1, comparison);
        }

        /// <summary>
        /// Gets the substring that comes after the first occurance of the match, or null if the match isn't found.
        /// </summary>
        public static string After(this string value, string match, bool includeMatch, StringComparison comparison)
        {
            return After(value, match, 1, includeMatch, comparison);
        }

        /// <summary>
        /// Gets the substring that comes after the Xth occurance of the match, or null if the match isn't found.
        /// </summary>
        public static string After(this string source, string match, int matchOccurance)
        {
            return After(source, match, matchOccurance, DefaultComparisonForAfter);
        }

        /// <summary>
        /// Gets the substring that comes after the Xth occurance of the match, or null if the match isn't found.
        /// </summary>
        public static string After(this string source, string match, int matchOccurance, bool includeMatch)
        {
            return After(source, match, matchOccurance, includeMatch, DefaultComparisonForAfter);
        }

        /// <summary>
        /// Gets the string that comes after the last occurance of the match.  So "me me me you".AfterLast("me") == " you".
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string AfterLast(this string value, string match)
        {
            return value.AfterLast(match, DefaultComparisonForAfter);
        }

        /// <summary>
        /// Gets the string that comes after the last occurance of the match. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string AfterLast(this string value, string match, bool includeMatch)
        {
            return value.AfterLast(match, includeMatch, DefaultComparisonForAfter);
        }

        /// <summary>
        /// Gets the string that comes after the last occurance of the match. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string AfterLast(this string value, string match, StringComparison comparison)
        {
            return After(value, match, 0, comparison);
        }

        /// <summary>
        /// Gets the string that comes after the last occurance of the match. 
        /// </summary>
        public static string AfterLast(this string value, string match, bool includeMatch, StringComparison comparison)
        {
            return After(value, match, 0, includeMatch, comparison);
        }

        /// <summary>
        /// Gets the substring that comes after the Xth occurance of the match, or null if the match isn't found.
        /// </summary>
        /// <param name="source">The string whose substring will be returned.</param>
        /// <param name="match">The string to find within the source string.</param>
        /// <param name="matchOccurance">Indicates after which occurance of the match string will be returned.  0 or less indicates the last occurance.  1 is the first, 2 is the second, etc.</param>
        /// <param name="comparison">The type of comparison when searching.</param>
        public static string After(this string source, string match, int matchOccurance, StringComparison comparison)
        {
            return After(source, match, matchOccurance, DefaultIncludeMatchForAfter, comparison);
        }

        /// <summary>
        /// Gets the substring that comes after the Xth occurance of the match, or null if the match isn't found.  Optionally includes match in the result.
        /// </summary>
        /// <param name="source">The string whose substring will be returned.</param>
        /// <param name="match">The string to find within the source string.</param>
        /// <param name="matchOccurance">Indicates after which occurance of the match string will be returned.  0 or less indicates the last occurance.  1 is the first, 2 is the second, etc.</param>
        /// <param name="includeMatch">If true, the match will be prepended to the result, meaning the result is after and including the match.  Default is false.</param>
        /// <param name="comparison">The type of comparison when searching.</param>
        public static string After(this string source, string match, int matchOccurance, bool includeMatch, StringComparison comparison)
        {
            if (!source.HasChars() || !match.HasChars())
                return source;

            var index = source.IndexOfOccurance(match, matchOccurance, comparison);
            return index < 0 ? null : (includeMatch ? source.Substring(index) : source.Substring(index + match.Length));
        }

        #endregion

        /// <summary>
        /// Returns the specified number of characters from the end of the string.  If the string is shorter than the number of chars specified, the entire string will be returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="charsToTake"></param>
        /// <returns></returns>
        public static string FromEnd(this string value, int charsToTake)
        {
            if (value == null)
                return null;
            if (charsToTake < 0)
                return null;
            if (charsToTake == 0)
                return string.Empty;

            int startIndex = Math.Max(value.Length - charsToTake, 0);
            return value.Substring(startIndex);
        }

        /// <summary>
        /// Returns this string with the given number of characters removed from the end of it.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string RemoveFromEnd(this string value, int chars)
        {
            if (chars < 0)
                throw new ArgumentOutOfRangeException("value", "chars must be >= 0");
            if (value == null)
                return null;

            if (chars == 0)
                return value;
            if (chars >= value.Length)
                return String.Empty;
            return value.Substring(0, value.Length - chars);
        }


        #endregion

        #region Combining

        /// <summary>
        /// If the value doesn't start with startWith, this adds it.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string StartWith(this string value, string prefix)
        {
            return value.StartWith(prefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// If the value doesn't start with startWith, this adds it.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string StartWith(this string value, string prefix, StringComparison comparisonType)
        {
            return value.StartWith(prefix, true, comparisonType);
        }


        /// <summary>
        /// Ensures the string starts with the given value.
        /// </summary>
        /// <param name="value">The string to potentially prepend startsWith.</param>
        /// <param name="prefix">The string that value must start with.</param>
        /// <param name="allowPartialMatch">If true "ttp".StartWith("http") will be "http".  If false it would be "httpttp".  So the long explanation is this specifies whether if the value starts with a partial start of startsWith, that partial match is trimmed from the prepended value.</param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static string StartWith(this string value, string prefix, bool allowPartialMatch, StringComparison comparisonType)
        {
            if (prefix == null)
                return value;
            if (string.IsNullOrEmpty(value))
                return prefix;
            if (value.StartsWith(prefix, comparisonType))
                return value;

            if (allowPartialMatch)
            {
                //start with the biggest non-full match and work backwards.  it doesen't already start with it, so you can skip the first letter
                for (var i = 1; i < prefix.Length; i++)
                {
                    var partialMatch = prefix.FromEnd(prefix.Length - i);
                    if ( value.StartsWith(partialMatch, comparisonType))
                    {
                        return string.Concat(prefix.Substring(0, i), value);
                    }
                }
            }

            return string.Concat(prefix, value);
        }

        /// <summary>
        /// Ensures the string doesn't begin with the given prefix.
        /// </summary>
        /// <remarks>
        /// It will repeat until it no longers starts with it, so "AAAB".NotStartingWith("A") == "B".
        /// </remarks>
        public static string StartWithout(this string value, string prefix)
        {
            return value.StartWithout(prefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Ensures the string doesn't begin with the given prefix.
        /// </summary>
        /// <remarks>
        /// It will repeat until it no longers starts with it, so "AAAB".NotStartingWith("A") == "B".
        /// </remarks>
        public static string StartWithout(this string value, string prefix, StringComparison comparisonType)
        {
            if (!value.HasChars() || !prefix.HasChars())
                return value;

            return value.StartsWith(prefix, comparisonType)
                       ? value.Substring(prefix.Length).StartWithout(prefix, comparisonType)
                       : value;
        }

        /// <summary>
        /// If the value doesn't end with endWith, this adds it.  If it already ends with part of endsWith, only the remaining substring is added.
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string EndWith(this string value, string suffix)
        {
            return value.EndWith(suffix, StringComparison.Ordinal);
        }

        /// <summary>
        /// If the value doesn't end with endWith, this adds it.  If it already ends with part of endsWith, only the remaining substring is added.
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public static string EndWith(this string value, string suffix, StringComparison comparisonType)
        {
            return value.EndWith(suffix, true, comparisonType);
        }

        /// <summary>
        /// Ensures the string ends with the given value.
        /// </summary>
        /// <param name="value">The string to potentially append endsWith to.</param>
        /// <param name="suffix">The string that value must end with.</param>
        /// <param name="allowPartialMatch">If true "hi jim".EndWith("jimmy") will be "hi jimmy".  If false it would be "hi jimjimmy".  So the long explanation is this specifies whether if the value ends with a partial start of endsWith, that partial match is trimmed from the appended value.</param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static string EndWith(this string value, string suffix, bool allowPartialMatch, StringComparison comparisonType)
        {
            if (suffix == null)
                return value;
            if (string.IsNullOrEmpty(value))
                return suffix;
            if (value.EndsWith(suffix, comparisonType))
                return value;

            if (allowPartialMatch)
            {
                //start with the biggest non-full match and work backwards
                for( var i = 1; i < suffix.Length; i++ )
                {
                    var partialMatch = suffix.RemoveFromEnd(i);
                    if ( value.EndsWith(partialMatch, comparisonType))
                    {
                        return string.Concat(value, suffix.FromEnd(i));
                    }
                }
            }

            return string.Concat(value, suffix);
        }


        /// <summary>
        /// If the value ends with endWith, this removes it.  It will repeat until it no longers ends with it, so "ABBB".NotEndingWith("B") == "A", "tr".NotEndingWith("a") == ""
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string EndWithout(this string value, string suffix)
        {
            return value.EndWithout(suffix, StringComparison.Ordinal);
        }

        /// <summary>
        /// If the value ends with endWith, this removes it.
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string EndWithout(this string value, string suffix, StringComparison comparisonType)
        {
            if (!value.HasChars() || !suffix.HasChars())
                return value;

            return value.EndsWith(suffix, comparisonType)
                       ? value.RemoveFromEnd(suffix.Length).EndWithout(suffix, comparisonType)
                       : value;
        }

        /// <summary>
        /// If the value doesn't start and end with surroundedBy, this adds it to the necessary sides.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string Surround(this string value, string surroundedBy)
        {
            return value.Surround(surroundedBy, StringComparison.Ordinal);
        }

        /// <summary>
        /// If the value doesn't start and end with surroundedBy, this adds it to the necessary sides.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string Surround(this string value, string surroundedBy, StringComparison comparisonType)
        {
            if (value == null)
                return null;
            return value.Surround(surroundedBy, surroundedBy, comparisonType);
        }

        /// <summary>
        /// If the value doesn't start and end with the values, this adds it to the necessary sides.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string Surround(this string value, string startingWith, string endingWith)
        {
            if (value == null)
                return null;
            return value.Surround(startingWith, endingWith, StringComparison.Ordinal);
        }

        /// <summary>
        /// If the value doesn't start and end with the values, this adds it to the necessary sides.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string Surround(this string value, string startingWith, string endingWith,
                                      StringComparison comparisonType)
        {
            if (value == null)
                return null;
            return value.StartWith(startingWith, comparisonType).EndWith(endingWith, comparisonType);
        }


        /// <summary>
        /// Makes sure the string doesn't start or end with the given value.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string NotSurroundedBy(this string value, string notSurroundedBy)
        {
            return value.NotSurroundedBy(notSurroundedBy, StringComparison.Ordinal);
        }

        /// <summary>
        /// Makes sure the string doesn't start or end with the given value.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string NotSurroundedBy(this string value, string notSurroundedBy, StringComparison comparisonType)
        {
            return value.NotSurroundedBy(notSurroundedBy, notSurroundedBy, comparisonType);
        }

        /// <summary>
        /// Makes sure the string doesn't start or end with the given value.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string NotSurroundedBy(this string value, string notStartingWith, string notEndingWith)
        {
            return value.NotSurroundedBy(notStartingWith, notEndingWith, StringComparison.Ordinal);
        }

        /// <summary>
        /// Makes sure the string doesn't start or end with the given value.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string NotSurroundedBy(this string value, string notStartingWith, string notEndingWith,
                                             StringComparison comparisonType)
        {
            if (value == null)
                return null;
            if (string.IsNullOrEmpty(notStartingWith))
                throw new ArgumentNullException("notStartingWith");
            if (string.IsNullOrEmpty(notEndingWith))
                throw new ArgumentNullException("notEndingWith");

            return value.StartWithout(notStartingWith, comparisonType).EndWithout(notEndingWith, comparisonType);
        }




        /// <summary>
        /// Assembles a list in the standard English style, like "one, two, and three" or "one and two"
        /// </summary>
        /// <param name="elements">The string elements to include in the list.</param>
        /// <returns></returns>
        /// <example>Passing in {"1","2","3"} will return "1, 2, and 3".</example>
        public static string AssembleList(this string[] elements)
        {
            return AssembleList(elements, ", ", ", and ", " and ");
        }


        /// <summary>
        /// Assembles a list with specified elemets and separators.
        /// </summary>
        /// <param name="elements">The string elements to include in the list.</param>
        /// <param name="separator">Separator between elements.</param>
        /// <param name="finalSeparator">Separator between the last two elements.</param>
        /// <param name="separatorForListOfTwo">Separator between elements if the list only has two elements.</param>
        /// <returns></returns>
        public static string AssembleList(this string[] elements, string separator, string finalSeparator,
                                          string separatorForTwoElements)
        {
            //set the final and listoftwo separators
            if (string.IsNullOrEmpty(finalSeparator)) finalSeparator = separator;
            if (string.IsNullOrEmpty(separatorForTwoElements)) separatorForTwoElements = finalSeparator;

            return elements.Join(separatorIndex =>
            {
                if (elements.Length == 2)
                    return separatorForTwoElements;
                if (separatorIndex < elements.Length - 2)
                    return finalSeparator;
                return separator;
            });
        }


        /// <summary>
        /// Shortcut for string.Join(string.Empty,values).  Instead you just write values.Join().  
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> values)
        {
            return values.Join(string.Empty);
        }

        /// <summary>
        /// Shortcut for string.Join(separator,values).  Instead you just write values.Join(separator).  
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values.ToArray());
        }

        /// <summary>
        /// Like String.Join but includes the ability to include custom separators.  It is useful for assembling lists like "Me, Myself, and I"
        /// </summary>
        /// <param name="values"></param>
        /// <param name="separator">Function that recieves the current index of the string in the list of values.  Returns the separator.</param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> values, Func<int, string> separator)
        {
            if (values == null)
                return null;

            StringBuilder sb = null;
            values.Each((str, index) =>
            {
                if (sb == null)
                    sb = new StringBuilder();
                else if (separator != null)
                {
                    string strSeparator = separator(index);
                    if (strSeparator != null)
                        sb.Append(strSeparator);
                }
                sb.Append(str);
            });

            return sb == null ? null : sb.ToString();
        }


        /// <summary>
        /// Takes this string and joins it with the other one, separated by the given separator.  It will guarantee that the separator only occurs once, even if this string ends with it and the other starts wtih it already.  This is useful in things like combining relative folders paths.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <param name="otherValue"></param>
        /// <returns></returns>
        public static string SeparatedBy(this string value, string separator, string otherValue)
        {
            return value.EndWithout(separator) + otherValue.StartWith(separator);
        }

        /// <summary>
        /// Repeats a certain string x number of times.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string Repeat(this string value)
        {
            return Repeat(value, 1);
        }

        /// <summary>
        /// Repeats a certain string x number of times.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string Repeat(this string value, int times)
        {
            if (times < 0)
                throw new ArgumentOutOfRangeException("times", "times cannot be less than 1");
            if (times == 0 || string.IsNullOrEmpty(value))
                return value;

            string[] repetitions = new string[times + 1];
            for (var i = 0; i <= times; i++)
            {
                repetitions[i] = value;
            }
            return string.Join(string.Empty, repetitions);
        }

        #endregion

        #region Hashing

        /// <summary>
        /// Generates a one-way hash for this plain text value with the key (a secret key) provided and returns a base64-encoded result.  Uses the system's default hash algorithm,  which in testing was shown to be HMACSHA1 (this will vary though).
        /// </summary>
        /// <param name="text">Plain text to be hased.</param>
        /// <param name="hashKey">The key, which should be kept secret, used to hash the text.</param>
        /// <returns>
        /// Hash value formatted as a base64-encoded string.
        /// </returns>
        public static string Hash(this string text, string hashKey)
        {
            return text.Hash(hashKey, null);
        }

        /// <summary>
        /// Generates a one-way hash for this plain text value with the key (a secret key) provided and returns a base64-encoded result.
        /// </summary>
        /// <param name="text">Plain text to be hased.</param>
        /// <param name="hashKey">The key, which should be kept secret, used to hash the text.</param>
        /// <param name="hashAlgorithm">Name of the hash algorithm, which is name of class in Crytography namespace.  Ex: "HMACSHA1".  If null or empty string is passed, the default hash algorithm will be used.</param>
        /// <returns>
        /// Hash value formatted as a base64-encoded string.
        /// </returns>
        public static string Hash(this string text, string hashKey, string hashAlgorithm)
        {
            return text.Hash(hashKey, hashAlgorithm, Encoding.UTF8);
        }

        /// <summary>
        /// Generates a one-way hash for this plain text value with the key (a secret key) provided and returns a base64-encoded result.
        /// </summary>
        /// <param name="text">Plain text to be hased.</param>
        /// <param name="hashKey">The key, which should be kept secret, used to hash the text.</param>
        /// <param name="hashAlgorithm">Name of the hash algorithm, which is name of class in Crytography namespace.  Ex: "HMACSHA1".  If null or empty string is passed, the default hash algorithm will be used.</param>
        /// <param name="encoding">The encoding that is used to feed the text value into the has algorithm.</param>
        /// <returns>
        /// Hash value formatted as a base64-encoded string.
        /// </returns>
        public static string Hash(this string text, string hashKey, string hashAlgorithm, Encoding encoding)
        {
            if (string.IsNullOrEmpty(hashKey))
            {
                throw new ArgumentException("hashKey is required.");
            }
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            KeyedHashAlgorithm hasher = string.IsNullOrEmpty(hashAlgorithm)
                                            ? KeyedHashAlgorithm.Create()
                                            : KeyedHashAlgorithm.Create(hashAlgorithm);
            if (hasher == null)
                throw new ArgumentException("Invalid hash algorithm passed.");
            hasher.Key = encoding.GetBytes(hashKey);
            byte[] hashed = hasher.ComputeHash(encoding.GetBytes(text.ToCharArray()));
            return Convert.ToBase64String(hashed);
        }

        /// <summary>
        /// Creates an MD5 hash, which is used across many platforms and is helpful for things like verifying a signature of an http request.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HashMd5(string input)
        {
            using (var md5Provider = new MD5CryptoServiceProvider())
            {
                return string.Join(string.Empty, md5Provider.ComputeHash(Encoding.UTF8.GetBytes(input)).Select(b => b.ToString("x2").ToLowerInvariant()).ToArray());
            }
        }


        /// <summary>
        /// Checks whether this string, which should plain text and not hashed, matches the version provided that has previously been hashed (preferrably through the Hash method).
        /// </summary>
        /// <param name="plainText">The plain, nonhashed text.</param>
        /// <param name="hasedValue">The previously hashed value to match against.</param>
        /// <param name="hashKey">The key, which should be kept secret, used to hash the text.</param>
        /// <param name="hashAlgorithm">Name of the hash algorithm, which is name of class in Crytography namespace.  Ex: "HMACSHA1".  If null or empty string is passed, the default hash algorithm will be used.</param>
        /// <returns></returns>
        public static bool MatchesHash(this string plainText, string hasedValue, string hashKey, string hashAlgorithm)
        {
            string hashed = plainText.Hash(hashKey, hashAlgorithm);
            return hashed.Equals(hasedValue, StringComparison.Ordinal);
        }

        #endregion

        /// <summary>
        /// Gets the index of the space before the closest word in the string from searchStartIndex.  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchStartIndex"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        private static int FindWordBreak(this string value, int searchStartIndex)
        {
            if (value == null)
                return -1;
            //todo: not all cultures use word break, but I couldn't find a framework method to handle it
            int nextWordStart = value.IndexOf(' ', searchStartIndex);
            //gets the position of the next space from searchStartIndex
            int previousWordStart = value.Substring(0, searchStartIndex).LastIndexOf(' ');
            //gets the position of the first previous space from searchStartIndex

            //no spaces were found, so return the value's length
            if (nextWordStart < 0 && previousWordStart < 0)
                return -1;

            if (nextWordStart < 0)
                return previousWordStart;
            if (previousWordStart < 0)
                return nextWordStart;

            //the next word starts further from the start index as the previous word, so previous word is closest
            if ((nextWordStart - searchStartIndex) > (searchStartIndex - previousWordStart))
            {
                return previousWordStart;
            }
            return nextWordStart;
        }

        /// <summary>
        /// Gets the index within the string of X occurance of a given substring.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="substring"></param>
        /// <param name="occuranceNumber">If 0 or less, this will indicate the last occurance.  Otherwise it specifies which # occurace.</param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        /// <remarks>This forms the basis for lots of the function, like Before and After.</remarks>
        public static int IndexOfOccurance(this string source, string substring, int occuranceNumber, StringComparison comparison)
        {
            if (source == null || !substring.HasChars())
                return -1;

            var index = int.MinValue;
            //check for last occurance
            if (occuranceNumber <= 0)
            {
                return source.LastIndexOf(substring, comparison);
            }
            for (var i = 0; i < occuranceNumber; i++)
            {
                index = source.IndexOf(substring, i == 0 ? 0 : index + substring.Length, comparison);
                if (index < 0)
                    return -1;
            }
            return index;
        }


        /// <summary>
        /// Takes the string and using TypeConverters, converts it to the given type.  A nice way to succinctly convert a string into any type.  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this string value)
        {
            if (value == null)
                return default(T);
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof (T));
                return (T) converter.ConvertFromInvariantString(value);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while converting '" + value + "' to " + typeof (T).FullName, ex);
            }
        }

        #region Advanced Format

        private const char OpeningFormatDelimiter = '{';
        private const char ClosingFormatDelimiter = '}';
        private static readonly Object ParsedFormatStringsLock = new Object();

        private static readonly Dictionary<string, Fragment[]> ParsedFormatStrings =
            new Dictionary<string, Fragment[]>(StringComparer.Ordinal);

        /// <summary>
        /// An advanced version of string.Format.  If you pass a primitive object (string, int, etc), it acts like the regular string.Format.  If you pass an anonmymous type, you can name the paramters by property name.
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <example>
        /// "The {Name} family has {Children} children".Format(new { Children = 4, Name = "Smith" })
        /// 
        /// results in 
        /// "This Smith family has 4 children
        /// </example>
        public static string Format(this string formatString, object arg, IFormatProvider format = null)
        {
            if (arg == null)
                return formatString;

            Type type = arg.GetType();
            if (Type.GetTypeCode(type) != TypeCode.Object || type.IsPrimitive)
                return string.Format(format, formatString, arg);

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(arg);
            return formatString.Format(property =>
                                           {
                                               object value = properties[property].GetValue(arg);
                                               return Convert.ToString(value, format);
                                           });
        }

        public static string Format(this string formatString, IDictionary<string, object> values,
                                    IFormatProvider format = null)
        {
            if (values == null)
                return formatString;
            return formatString.Format(token => { return Convert.ToString(values[token], format); });
        }

        public static string Format(this string formatString, Func<string, string> formatFragmentHandler)
        {
            return formatString.Format(formatFragmentHandler, v => v);
        }

        public static string Format(this string formatString, Func<string, string> formatFragmentHandler,
                                    Func<string, string> formatLiteralHandler)
        {
            if (string.IsNullOrEmpty(formatString))
                return formatString;
            Fragment[] fragments = GetParsedFragments(formatString);
            if (fragments == null || fragments.Length == 0)
                return formatString;

            return string.Join(string.Empty, fragments.Select(fragment =>
                                                                  {
                                                                      if (fragment.Type == FragmentType.Literal)
                                                                          return formatLiteralHandler(fragment.Value);
                                                                      return formatFragmentHandler(fragment.Value);
                                                                  }).ToArray());
        }

        private static Fragment[] GetParsedFragments(string formatString)
        {
            Fragment[] fragments;
            if (ParsedFormatStrings.TryGetValue(formatString, out fragments))
            {
                return fragments;
            }
            lock (ParsedFormatStringsLock)
            {
                if (!ParsedFormatStrings.TryGetValue(formatString, out fragments))
                {
                    fragments = Parse(formatString);
                    ParsedFormatStrings.Add(formatString, fragments);
                }
            }
            return fragments;
        }

        /// <summary>
        /// Parses the given format string into a list of fragments.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private static Fragment[] Parse(string format)
        {
            int lastCharIndex = format.Length - 1;
            int currFragEndIndex;
            Fragment currFrag = ParseFragment(format, 0, out currFragEndIndex);

            if (currFragEndIndex == lastCharIndex)
            {
                return new[] {currFrag};
            }

            var fragments = new List<Fragment>();
            while (true)
            {
                fragments.Add(currFrag);
                if (currFragEndIndex == lastCharIndex)
                {
                    break;
                }
                currFrag = ParseFragment(format, currFragEndIndex + 1, out currFragEndIndex);
            }
            return fragments.ToArray();
        }

        /// <summary>
        /// Finds the next delimiter from the starting index.
        /// </summary>
        private static Fragment ParseFragment(string format, int startIndex, out int fragmentEndIndex)
        {
            bool foundEscapedDelimiter = false;
            FragmentType type = FragmentType.Literal;

            int numChars = format.Length;
            for (int i = startIndex; i < numChars; i++)
            {
                char currChar = format[i];
                bool isOpenBrace = currChar == OpeningFormatDelimiter;
                bool isCloseBrace = isOpenBrace ? false : currChar == ClosingFormatDelimiter;

                if (!isOpenBrace && !isCloseBrace)
                {
                    continue;
                }
                if (i < (numChars - 1) && format[i + 1] == currChar)
                {
//{{ or }}
                    i++;
                    foundEscapedDelimiter = true;
                }
                else if (isOpenBrace)
                {
                    if (i == startIndex)
                    {
                        type = FragmentType.FormatItem;
                    }
                    else
                    {
                        if (type == FragmentType.FormatItem)
                            throw new FormatException(
                                "Two consequtive unescaped { format item openers were found.  Either close the first or escape any literals with another {.");

                        //curr character is the opening of a new format item.  so we close this literal out
                        string literal = format.Substring(startIndex, i - startIndex);
                        if (foundEscapedDelimiter)
                            literal = ReplaceEscapes(literal);

                        fragmentEndIndex = i - 1;
                        return new Fragment(FragmentType.Literal, literal);
                    }
                }
                else
                {
//close bracket
                    if (i == startIndex || type == FragmentType.Literal)
                        throw new FormatException("A } closing brace existed without an opening { brace.");

                    string formatItem = format.Substring(startIndex + 1, i - startIndex - 1);
                    if (foundEscapedDelimiter)
                        formatItem = ReplaceEscapes(formatItem);
                    //a format item with a { or } in its name is crazy but it could be done
                    fragmentEndIndex = i;
                    return new Fragment(FragmentType.FormatItem, formatItem);
                }
            }

            if (type == FragmentType.FormatItem)
                throw new FormatException("A format item was opened with { but was never closed.");

            fragmentEndIndex = numChars - 1;
            string literalValue = format.Substring(startIndex);
            if (foundEscapedDelimiter)
                literalValue = ReplaceEscapes(literalValue);

            return new Fragment(FragmentType.Literal, literalValue);
        }

        /// <summary>
        /// Replaces escaped brackets, turning '{{' and '}}' into '{' and '}', respectively.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ReplaceEscapes(string value)
        {
            return value.Replace("{{", "{").Replace("}}", "}");
        }

        #region Nested type: Fragment

        private class Fragment
        {
            public Fragment(FragmentType type, string value)
            {
                Type = type;
                Value = value;
            }

            public FragmentType Type { get; private set; }

            /// <summary>
            /// The literal value, or the name of the fragment, depending on fragment type.
            /// </summary>
            public string Value { get; private set; }
        }

        #endregion

        #region Nested type: FragmentType

        private enum FragmentType
        {
            Literal,
            FormatItem
        }

        #endregion

        #endregion
    }


    /// <summary>
    /// Specifies how to trim charachters from `a string that doesn't fit a length specified during Truncate().  Inspired by StringTruncating
    /// </summary>
    public enum StringTruncating
    {
        /// <summary>
        /// Specifies that the text is trimmed to the nearest character.
        Character,

        /// <summary>
        /// Specifies that text is trimmed to the nearest word.  This assumes words are separated by a single space, which may not apply to some cases or cultures.
        /// </summary>
        Word,

        /// <summary>
        /// Specifies that the text is trimmed to the nearest character, and an ellipsis is inserted at the end of a trimmed line.
        /// </summary>
        EllipsisCharacter,

        /// <summary>
        /// Specifies that text is trimmed to the nearest word, and an ellipsis is inserted at the end of a trimmed line.
        /// </summary>
        EllipsisWord,

        /// <summary>
        //  The front and end of the string is left and the ellipsis is put in the middle.
        /// </summary>
        EllipsisCenter
    }


    public enum CharsThatMatter
    {
        /// <summary>
        /// Any character counts.
        /// </summary>
        Any,

        /// <summary>
        /// Everything that isn't whitespace is counted.
        /// </summary>
        NonWhitespace,

        /// <summary>
        /// Only letters are counted.
        /// </summary>
        Letters,

        /// <summary>
        /// Only letters and digits counted.
        /// </summary>
        LettersAndDigits,

        /// <summary>
        /// Only digits are counted.
        /// </summary>
        Digits,
    }
}