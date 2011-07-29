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
        private static readonly Regex alphaNumeric = new Regex("[a-zA-Z0-9]");
        private static readonly Regex alpha = new Regex("[a-zA-Z]");
        private static readonly Regex numericNoDecimal = new Regex("[0-9]");
        private static readonly Regex numericDecimal = new Regex(@"[0-9\.]");

        /// <summary>
        /// Whether the value is not null and has at least one character.  It's the exact same thing as writing the clunky !string.IsNullOrEmpty(value).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasChars(this string value)
        {
            return value.HasChars(CharsThatMatter.Any);
        }

        /// <summary>
        /// Whether the value is not null and has at least one character, with an option to disregard whitespace.
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
        /// Just a shortcut for Equals(StringComparison.Ordinal).  Slightly shorter and neater.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
//todo: check this        [Obsolete("Normal string.Equals(string) does this anyway.  Whoops!")]
        public static bool EqualsExact(this string value, string match)
        {
            if (value == null)
                return match == null;
            if (match == null)
                return false;
            return value.Equals(match, StringComparison.Ordinal);
        }

        /// <summary>
        /// Just a shortcut for Equals(StringComparison.Ordinal).  Slightly shorter and neater.
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
        /// Removes all occurances of the given string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toRemove"></param>
        /// <returns></returns>
        public static string Strip(this string value, string toRemove)
        {
            if (value == null)
                return null;
            return value.Replace(toRemove, String.Empty);
        }


        /// <summary>
        /// Gets the string that comes before the first occurance of the match is found.  For example, calling "I eat hot dog".Before("hot") would return "i eat ".
        /// Returns null if the match wasn't found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string Before(this string value, string match)
        {
            return value.Before(match, StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the string that comes before the first occurance of the match is found.  For example, calling "I eat hot dog".Before("hot") would return "i eat ".
        /// Returns null if the match wasn't found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string Before(this string value, string match, StringComparison comparison)
        {
            if (value == null)
                return null;
            var index = value.IndexOf(match, 0, comparison);
            if (index >= 0)
            {
                return value.Substring(0, index);
            }
            return null;
        }


        /// <summary>
        /// Gets the string that comes after the last occurance of the match is found.  For example, calling "I eat hot dog".After("hot") would return "i eat ".
        /// Returns null if the match wasn't found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string After(this string value, string match)
        {
            return value.After(match, StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the string that comes after the first occurance of the match is found.  For example, calling "I eat hot dog".After("hot") would return " dog".
        /// Returns null if the match wasn't found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string After(this string value, string match, StringComparison comparison)
        {
            return After(value, match, 1, false, comparison);
        }

        /// <summary>
        /// Gets the substring of source that comes after an occurance of the match string.
        /// </summary>
        /// <param name="source">The string whose substring will be returned.</param>
        /// <param name="match">The string to find within the source string.</param>
        /// <param name="matchOccurance">Indicates after which occurance of the match string will be returned.  <= 0 indicates the last occurance.  1 is the first, 2 is the second, etc.</param>
        /// <param name="prependMatch">When true, the match will be prepended to the result.  The prepended value is taken from the original string, not the actual "match" property.  This matters when ignoring case.  Like "very good".After("VERY") with match prepended will return "very good".</param>
        /// <param name="comparison">The type of comparison when searching through files.</param>
        /// <returns></returns>
        public static string After(this string source, string match, int matchOccurance, bool prependMatch,
                                   StringComparison comparison)
        {
            if (source == null)
            {
                return null;
            }
            //nothing to match so just return the source string.
            if (!match.HasChars())
            {
                return source;
            }
            var index = int.MinValue;
            //check for last occurance
            if (matchOccurance <= 0)
            {
                index = source.LastIndexOf(match, comparison);
                if (index < 0)
                    return null;
                return prependMatch ? source.Substring(index) : source.Substring(index + match.Length);
            }
            for (var i = 0; i < matchOccurance; i++)
            {
                index = source.IndexOf(match, i == 0 ? 0 : index + match.Length, comparison);
                if (index < 0)
                    return null;
            }
            return prependMatch ? source.Substring(index) : source.Substring(index + match.Length);
        }

        public static string AfterLast(this string value, string match)
        {
            return value.AfterLast(match, StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the string that comes after the first occurance of the match is found.  For example, calling "I eat hot dog".After("hot") would return " dog".
        /// Returns null if the match wasn't found.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string AfterLast(this string value, string match, StringComparison comparison)
        {
            return After(value, match, 0, false, comparison);
        }

        /// <summary>
        /// Gets the string that comes after the beginning the first occurance of the given match.  For example, "you didn't know".From("did") == "didn't know".  Uses case-sensitive ordinal search.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        /// <remarks>A good opposite to this could be Until, although that's not right (Up to and including...find a one word answer for that)</remarks>
        public static string From(this string value, string match)
        {
            return value.From(match, StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the string that comes after the beginning the first occurance of the given match.  For example, "you didn't know".From("did") == "didn't know"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        /// <remarks>A good opposite to this could be Until, although that's not right (Up to and including...find a one word answer for that)</remarks>
        public static string From(this string value, string match, StringComparison comparison)
        {
            return After(value, match, 1, true, comparison);
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
                throw new ArgumentOutOfRangeException("value","chars must be >= 0");
            if (value == null)
                return null;

            if (chars == 0)
                return value;
            if (chars >= value.Length)
                return String.Empty;
            return value.Substring(0, value.Length - chars);
        }


        /// <summary>
        /// If the value doesn't start with startWith, this adds it.
        /// </summary>
        /// <param name="startsWith"></param>
        /// <returns></returns>
        public static string StartWith(this string value, string startsWith)
        {
            return value.StartWith(startsWith, StringComparison.Ordinal);
        }

        /// <summary>
        /// If the value doesn't start with startWith, this adds it.
        /// </summary>
        /// <param name="startsWith"></param>
        /// <returns></returns>
        public static string StartWith(this string value, string startsWith, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(value))
                return startsWith;
            return value.StartsWith(startsWith, comparisonType) ? value : string.Concat(startsWith, value);
        }

        /// <summary>
        /// If the value starts with startWith, this removes it.  It will repeat until it no longers starts with it, so "AAAB".NotStartingWith("A") == "B"
        /// </summary>
        /// <param name="startsWith"></param>
        /// <returns></returns>
        public static string StartWithout(this string value, string startsWith)
        {
            return value.StartWithout(startsWith, StringComparison.Ordinal);
        }

        /// <summary>
        /// If the value starts with startWith, this removes it.  It will repeat until it no longers starts with it, so "AAAB".NotStartingWith("A") == "B"
        /// </summary>
        /// <param name="startsWith"></param>
        /// <returns></returns>
        public static string StartWithout(this string value, string startsWith, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.StartsWith(startsWith, comparisonType)
                       ? value.Substring(startsWith.Length).StartWithout(startsWith, comparisonType)
                       : value;
        }


        /// <summary>
        /// If the value doesn't end with endWith, this adds it.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string EndWith(this string value, string endsWith)
        {
            return value.EndWith(endsWith, StringComparison.Ordinal);
        }

        /// <summary>
        /// If the value doesn't end with endWith, this adds it.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        /// <remarks>
        /// todo: There is a problem with this.  "/te".EndingWith("/test") will create "/te/test", which it shouldn't do.  
        /// </remarks>
        public static string EndWith(this string value, string endsWith, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(value))
                return endsWith;
            return value.EndsWith(endsWith, comparisonType) ? value : string.Concat(value, endsWith);
        }


        /// <summary>
        /// If the value ends with endWith, this removes it.  It will repeat until it no longers ends with it, so "ABBB".NotEndingWith("B") == "A", "tr".NotEndingWith("a") == ""
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string EndWithout(this string value, string endsWith)
        {
            return value.EndWithout(endsWith, StringComparison.Ordinal);
        }

        /// <summary>
        /// If the value ends with endWith, this removes it.  It will repeat until it no longers ends with it, so "AAAB".NotEndingWith("A") == "B"
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string EndWithout(this string value, string endsWith, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.EndsWith(endsWith, comparisonType)
                       ? value.RemoveFromEnd(endsWith.Length).EndWithout(endsWith, comparisonType)
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
        /// Splits the string into its given lines.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="doubleQuotes"></param>
        /// <returns></returns>
        public static string[] SplitLines(this string value)
        {
            return value.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
                //will handle multiple types of line breaks.  found at http://stackoverflow.com/questions/1547476/easiest-way-to-split-a-string-on-newlines-in-net
        }

        /// <summary>
        /// If the value has characters, it returns the string.  If null or empty, it returns null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CharsOrNull(this string value)
        {
            return value.CharsOr((string) null);
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


        public static string Truncate(this string value, int maxLength, StringTruncating trimMode)
        {
            return Truncate(value, maxLength, trimMode, "...");
        }


        /// <summary>
        /// If the length of this string exceeds the max, it will be trimmed according to the type specified.
        /// </summary>
        /// <param name="value">The value to be truncated.</param>
        /// <param name="maxLength">The max length of the string.</param>
        /// <param name="trimming"></param>
        /// <param name="ellipsis"></param>
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
                        int endLength = maxLength/2;
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
            return hashed.EqualsExact(hasedValue);
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
        public static string SeperatedBy(this string value, string separator, string otherValue)
        {
            return value.EndWithout(separator) + otherValue.StartWith(separator);
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