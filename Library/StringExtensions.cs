using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// Wetnap extension methods for strings.
    /// </summary>
    partial class WetnapExtensions
    {

        /// <summary>
        /// Whether the value is not null and has at least one character (whitespace or not).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasChars(this string value)
        {
            return value.HasChars(true);
        }


        /// <summary>
        /// Whether the value is not null and has at least one character, with an option to disregard whitespace.
        /// </summary>
        public static bool HasChars(this string value, bool whitespaceCounts)
        {
            return whitespaceCounts ? !string.IsNullOrEmpty(value) : !IsNullOrWhiteSpace(value);
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
        public static bool HasNonWhitespace(this string value)
        {
            return !IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// From .net 4.0, so we can use that method without requiring 4.0.  
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null && value.Length > 0)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Just a shortcut for Equals(StringComparison.Ordinal).  Slightly shorter and neater.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toRemove"></param>
        /// <returns></returns>
        public static bool EqualsExact(this string value, string match)
        {
            if (value == null)
                return match == null;
            else if (match == null)
                return false;
            return value.Equals(match, StringComparison.Ordinal);
        }

        /// <summary>
        /// Just a shortcut for Equals(StringComparison.Ordinal).  Slightly shorter and neater.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toRemove"></param>
        /// <returns></returns>
        public static bool EqualsCaseInsensitive(this string value, string match)
        {
            if (value == null)
                return match == null;
            else if (match == null)
                return false;
            return value.Equals(match, StringComparison.OrdinalIgnoreCase);
        }


        /// <summary>
        /// Indicates whether the string passed in is equal to any of the possible matches provided.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toRemove"></param>
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
            foreach (var m in match)
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
            if (value == null)
                return null;
            var index = value.IndexOf(match, 0, comparison);
            if (index >= 0)
            {
                return value.Substring(index + match.Length);
            }
            return null;
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
            if (value == null)
                return null;
            var index = value.LastIndexOf(match, comparison);
            if (index >= 0)
            {
                return value.Substring(index + match.Length);
            }
            return null;
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
            if (value == null)
                return null;
            var index = value.IndexOf(match, 0, comparison);
            if (index >= 0)
            {
                return value.Substring(index);
            }
            return null;
        }


        /// <summary>
        /// Indicates whether the string has at least one letter.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasLetters(this string value)
        {
            if (value == null)
                return false;
            foreach (var currChar in value)
            {
                if (Char.IsLetter(currChar))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Removes every character from the string that is not a number.  so "4ms" will return "4" and "4 plus 1" returns "41".
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FilterDigits(this string value)
        {
            if (value == null)
                return null;
            return new string(value.ToCharArray().Where(o => Char.IsDigit(o)).ToArray());
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

            foreach (var match in matches)
            {
                var index = value.IndexOf(match, 0, comparison);
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
            return System.Convert.ToInt32(value);
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
            var firstChar = char.ToLower(value[0]);
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

            var startIndex = Math.Max(value.Length - charsToTake, 0);
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
                throw new ArgumentOutOfRangeException("chars must be >= 0");
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
            return value.StartsWith(startsWith, comparisonType) ? value.Substring(startsWith.Length).StartWithout(startsWith, comparisonType) : value;
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
            return value.EndsWith(endsWith, comparisonType) ? value.RemoveFromEnd(endsWith.Length).EndWithout(endsWith, comparisonType) : value;
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
        public static string Surround(this string value, string startingWith, string endingWith, StringComparison comparisonType)
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
        public static string NotSurroundedBy(this string value, string notStartingWith, string notEndingWith, StringComparison comparisonType)
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
            var indent = "".PadLeft(spaces, ' ');
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
            return value.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);//will handle multiple types of line breaks.  found at http://stackoverflow.com/questions/1547476/easiest-way-to-split-a-string-on-newlines-in-net

        }

        /// <summary>
        /// If the value has characters, it returns the string.  If null or empty, it returns null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CharsOrNull(this string value)
        {
            return value.CharsOr(null);
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
        /// If the value has characters (!IsNullOrEmpty), it returns the original string.  If it is null or empty, it returns the "returnIfNoChars" value.
        /// </summary>
        public static string CharsOr(this string value, string returnIfNoChars)
        {
            if (string.IsNullOrEmpty(value))
                return returnIfNoChars;
            else
                return value;
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
                        int endLength = maxLength / 2;
                        int beginningLength = maxLength - endLength; //number to take from beginnning.  this is not trimlength/2 because trimlength may be odd

                        value = value.Substring(0, beginningLength) + ellipsis + value.Substring(value.Length - endLength);
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
            int nextWordStart = value.IndexOf(' ', searchStartIndex);//gets the position of the next space from searchStartIndex
            int previousWordStart = value.Substring(0, searchStartIndex).LastIndexOf(' '); //gets the position of the first previous space from searchStartIndex

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
            else
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
            var hasher = string.IsNullOrEmpty(hashAlgorithm) ? KeyedHashAlgorithm.Create() : KeyedHashAlgorithm.Create(hashAlgorithm);
            if (hasher == null)
                throw new ArgumentException("Invalid hash algorithm passed.");
            hasher.Key = encoding.GetBytes(hashKey);
            var hashed = hasher.ComputeHash(encoding.GetBytes(text.ToCharArray()));
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
            var hashed = plainText.Hash(hashKey, hashAlgorithm);
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
        public static string AssembleList(this string[] elements, string separator, string finalSeparator, string separatorForTwoElements)
        {
            //set the final and listoftwo separators
            if (string.IsNullOrEmpty(finalSeparator)) finalSeparator = separator;
            if (string.IsNullOrEmpty(separatorForTwoElements)) separatorForTwoElements = finalSeparator;

            return elements.Join((separatorIndex) =>
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
                    var strSeparator = separator(index);
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
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromInvariantString(value);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while converting '" + value + "' to " + typeof(T).FullName, ex);
            }
        }

        #region Advanced Format

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

            var type = arg.GetType();
            if (Type.GetTypeCode(type) != TypeCode.Object || type.IsPrimitive)
                return string.Format(format, formatString, arg);

            var properties = TypeDescriptor.GetProperties(arg);
            return formatString.Format((property) =>
            {
                var value = properties[property].GetValue(arg);
                return Convert.ToString(value, format);
            });
        }

        public static string Format(this string formatString, IDictionary<string, object> values, IFormatProvider format = null)
        {
            if (values == null)
                return formatString;
            return formatString.Format((token) =>
            {
                return Convert.ToString(values[token], format);
            });
        }

        public static string Format(this string formatString, Func<string, string> formatFragmentHandler)
        {
            return formatString.Format(formatFragmentHandler, v => v);
        }

        public static string Format(this string formatString, Func<string, string> formatFragmentHandler, Func<string,string> formatLiteralHandler)
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
                else
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

        private static Object ParsedFormatStringsLock = new Object();
        private static Dictionary<string, Fragment[]> ParsedFormatStrings = new Dictionary<string, Fragment[]>(StringComparer.Ordinal);

        const char OpeningFormatDelimiter = '{';
        const char ClosingFormatDelimiter = '}';

        /// <summary>
        /// Parses the given format string into a list of fragments.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        static Fragment[] Parse(string format)
        {
            int lastCharIndex = format.Length - 1;
            int currFragEndIndex;
            Fragment currFrag = ParseFragment(format, 0, out currFragEndIndex);

            if (currFragEndIndex == lastCharIndex)
            {
                return new Fragment[] { currFrag };
            }

            List<Fragment> fragments = new List<Fragment>();
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
        static Fragment ParseFragment(string format, int startIndex, out int fragmentEndIndex)
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
                else if (i < (numChars - 1) && format[i + 1] == currChar)
                {//{{ or }}
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
                            throw new FormatException("Two consequtive unescaped { format item openers were found.  Either close the first or escape any literals with another {.");

                        //curr character is the opening of a new format item.  so we close this literal out
                        string literal = format.Substring(startIndex, i - startIndex);
                        if (foundEscapedDelimiter)
                            literal = ReplaceEscapes(literal);

                        fragmentEndIndex = i - 1;
                        return new Fragment(FragmentType.Literal, literal);
                    }
                }
                else
                {//close bracket
                    if (i == startIndex || type == FragmentType.Literal)
                        throw new FormatException("A } closing brace existed without an opening { brace.");

                    string formatItem = format.Substring(startIndex + 1, i - startIndex - 1);
                    if (foundEscapedDelimiter)
                        formatItem = ReplaceEscapes(formatItem);//a format item with a { or } in its name is crazy but it could be done
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
        static string ReplaceEscapes(string value)
        {
            return value.Replace("{{", "{").Replace("}}", "}");
        }

        private enum FragmentType
        {
            Literal,
            FormatItem
        }

        private class Fragment
        {

            public Fragment(FragmentType type, string value)
            {
                Type = type;
                Value = value;
            }

            public FragmentType Type
            {
                get;
                private set;
            }

            /// <summary>
            /// The literal value, or the name of the fragment, depending on fragment type.
            /// </summary>
            public string Value
            {
                get;
                private set;
            }


        }

        #endregion

    }



    /// <summary>
    /// Specifies how to trim charachters from a string that doesn't fit a length specified during Truncate().  Inspired by StringTruncating
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

}
