using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Steveo.Wetnap.Tests
{
    [TestClass]
    public class StringUnitTests
    {

        

        [TestMethod]
        public void StringHasChars()
        {
            Assert.IsTrue(" ".HasChars());
            Assert.IsTrue("d".HasChars());
            Assert.IsFalse("".HasChars());
            Assert.IsFalse(((string)null).HasChars());

            Assert.IsTrue(" ".HasChars(true));
            Assert.IsTrue(!" ".HasChars(false));

            Assert.IsTrue(" ".HasChars(CharsThatMatter.Any));
            Assert.IsTrue(" 2".HasChars(CharsThatMatter.Digits));
            Assert.IsTrue("2 a ".HasChars(CharsThatMatter.Letters));
            Assert.IsTrue(" a ".HasChars(CharsThatMatter.Letters));
            Assert.IsTrue(" 2".HasChars(CharsThatMatter.LettersAndDigits));
            Assert.IsTrue("a".HasChars(CharsThatMatter.LettersAndDigits));
            Assert.IsTrue("a2".HasChars(CharsThatMatter.LettersAndDigits));
            Assert.IsTrue("a".HasChars(CharsThatMatter.NonWhitespace));
            Assert.IsTrue("3".HasChars(CharsThatMatter.NonWhitespace));
            Assert.IsTrue(")".HasChars(CharsThatMatter.NonWhitespace));

            Assert.IsFalse("".HasChars(CharsThatMatter.Any));
            Assert.IsFalse(" t".HasChars(CharsThatMatter.Digits));
            Assert.IsFalse("2  ".HasChars(CharsThatMatter.Letters));
            Assert.IsFalse(" ( ".HasChars(CharsThatMatter.Letters));
            Assert.IsFalse(" $".HasChars(CharsThatMatter.LettersAndDigits));
            Assert.IsFalse(" ".HasChars(CharsThatMatter.LettersAndDigits));
            Assert.IsFalse(" ~".HasChars(CharsThatMatter.LettersAndDigits));
            Assert.IsFalse("  ".HasChars(CharsThatMatter.NonWhitespace));
            Assert.IsFalse("\r".HasChars(CharsThatMatter.NonWhitespace));
            Assert.IsFalse(Environment.NewLine.HasChars(CharsThatMatter.NonWhitespace));

            Assert.IsTrue(" ".HasChars(Char.IsWhiteSpace));
            Assert.IsFalse(" ".HasChars(Char.IsLetter));

            Assert.IsTrue(" 2 ".HasChars(Char.IsDigit));
            Assert.IsTrue("2".HasChars(Char.IsDigit));

            Assert.IsFalse("2".HasChars(Char.IsLetter));
            Assert.IsFalse("_".HasChars(Char.IsLetter));
        }

        [TestMethod]
        public void StringCharsOr()
        {
            string val = null;
            Assert.AreEqual(val.CharsOrNull(), null);
            Assert.AreEqual(val.CharsOrEmpty(), string.Empty);
            Assert.AreEqual(val.CharsOr("sweet"), "sweet");
            Assert.AreEqual(val.CharsOr(() => "sweet"), "sweet");

            val = string.Empty;
            Assert.AreEqual(val.CharsOrNull(), null);
            Assert.AreEqual(val.CharsOrEmpty(), string.Empty);
            Assert.AreEqual(val.CharsOr("sweet"), "sweet");
            Assert.AreEqual(val.CharsOr(() => "sweet"), "sweet");

            val = "hi";
            Assert.AreEqual(val.CharsOrNull(), "hi");
            Assert.AreEqual(val.CharsOrEmpty(), "hi");
            Assert.AreEqual(val.CharsOr("sweet"), "hi");
            Assert.AreEqual(val.CharsOr(() => "sweet"), "hi");

            Assert.AreEqual(val.CharsOr(CharsThatMatter.Digits, "sweet"), "sweet");
            Assert.AreEqual(val.CharsOr(CharsThatMatter.Digits, () => "sweet"), "sweet");
            Assert.AreEqual(val.CharsOr(CharsThatMatter.LettersAndDigits, "sweet"), val);
            Assert.AreEqual(val.CharsOr(CharsThatMatter.LettersAndDigits, () => "sweet"), val);

            Assert.AreEqual(val.CharsOr((c) => c == '5', "sweet"), "sweet");
            Assert.AreEqual(val.CharsOr((c) => c == '5', () => "sweet"), "sweet");
            Assert.AreEqual(val.CharsOr((c) => c == 'h', "sweet"), val);
            Assert.AreEqual(val.CharsOr((c) => c == 'h', () => "sweet"), val);
        }


        [TestMethod]
        public void StringBefore()
        {
            //Before(this string value, string match)
            Assert.AreEqual("big big big thing".Before("big"), string.Empty);
            Assert.AreEqual(((string)null).Before("REALLY"), null);
            Assert.AreEqual(string.Empty.Before("REALLY"), string.Empty);
            Assert.AreEqual("I'm okay, really, really".Before("REALLY"), null);
            Assert.AreEqual("I'm okay, really, really".Before("really"), "I'm okay, ");
            Assert.AreEqual("I'm okay, really, really".Before("'"), "I");
            Assert.AreEqual("I'm okay, really, really".Before("really,"), "I'm okay, ");

            //Before(this string source, string match, bool includeMatch)
            Assert.AreEqual("big big big thing".Before("BIG", true), null);
            Assert.AreEqual("big big big thing".Before("big", true), "big");
            Assert.AreEqual("big big big thing".Before("big", false), "");
            Assert.AreEqual("the big big big thing".Before("BIG", false), null);
            Assert.AreEqual("the big big big thing".Before("big", true), "the big");
            Assert.AreEqual("the big big big thing".Before("big", false), "the ");

            //Before(this string source, string match, StringComparison comparison)
            Assert.AreEqual("big big big thing".Before("BIG", StringComparison.Ordinal), null);
            Assert.AreEqual("big big big thing".Before("BIG", StringComparison.OrdinalIgnoreCase), "");
            Assert.AreEqual("the big big big thing".Before("BIG", StringComparison.Ordinal), null);
            Assert.AreEqual("the big big big thing".Before("BIG", StringComparison.OrdinalIgnoreCase), "the ");

            //Before(this string source, string match, bool includeMatch, StringComparison comparison)
            Assert.AreEqual("big big big thing".Before("BIG", true, StringComparison.Ordinal), null);
            Assert.AreEqual("big big big thing".Before("BIG", true, StringComparison.OrdinalIgnoreCase), "big");
            Assert.AreEqual("big big big thing".Before("BIG", false, StringComparison.OrdinalIgnoreCase), "");
            Assert.AreEqual("the big big big thing".Before("BIG", false, StringComparison.OrdinalIgnoreCase), "the ");
            Assert.AreEqual("the big big big thing".Before("BIG", true, StringComparison.OrdinalIgnoreCase), "the big");

            //Before(this string source, string match, int matchOccurance, StringComparison comparison)
            Assert.AreEqual("big big big thing".Before("BIG", 2, StringComparison.Ordinal), null);
            Assert.AreEqual("big big big thing".Before("BIG", 2, StringComparison.OrdinalIgnoreCase), "big ");

            //Before(this string source, string match, int matchOccurance, bool includeMatch)
            Assert.AreEqual("big big big thing".Before("big", 2, true), "big big");
            Assert.AreEqual("big big big thing".Before("big", 2, false), "big ");
            Assert.AreEqual("big big big thing".Before("BIG", 2, true), null);

            //Before(this string source, string match, int matchOccurance, bool includeMatch, StringComparison comparison)
            Assert.AreEqual("".Before("", 1, false, StringComparison.Ordinal), "");
            Assert.AreEqual("".Before(null, 1, false, StringComparison.Ordinal), "");
            Assert.AreEqual("hey".Before(null, 1, false, StringComparison.Ordinal), "hey");
            Assert.AreEqual("big big big thing".Before("big", 1, false, StringComparison.Ordinal), "");
            Assert.AreEqual("big big big thing".Before("big", 1, true, StringComparison.Ordinal), "big");
            Assert.AreEqual("big big big thing".Before("BIG", 1, false, StringComparison.Ordinal), null);
            Assert.AreEqual("big big big thing".Before("BIG", 1, true, StringComparison.Ordinal), null);
            Assert.AreEqual("big big big thing".Before("BIG", 1, false, StringComparison.OrdinalIgnoreCase), "");
            Assert.AreEqual("big big big thing".Before("BIG", 1, true, StringComparison.OrdinalIgnoreCase), "big");
            Assert.AreEqual("big big big thing".Before("big", 0, false, StringComparison.Ordinal), "big big ");
            Assert.AreEqual("big big big thing".Before("big", 0, true, StringComparison.Ordinal), "big big big");
            Assert.AreEqual("big big big thing".Before("BIG", 0, false, StringComparison.OrdinalIgnoreCase), "big big ");
            Assert.AreEqual("big big big thing".Before("BIG", 0, true, StringComparison.OrdinalIgnoreCase),
                            "big big big");
            Assert.AreEqual("big big big thing".Before("big", 2, false, StringComparison.Ordinal), "big ");
            Assert.AreEqual("big big big thing".Before("big", 2, true, StringComparison.Ordinal), "big big");
            Assert.AreEqual("big big big thing".Before("BIG", 2, false, StringComparison.OrdinalIgnoreCase), "big ");
            Assert.AreEqual("big big big thing".Before("BIG", 2, true, StringComparison.OrdinalIgnoreCase), "big big");
        }

        [TestMethod]
        public void StringBeforeLast()
        {
            //BeforeLast(this string value, string match)
            Assert.AreEqual("big big big thing".BeforeLast("big"), "big big ");
            Assert.AreEqual("big big big thing".BeforeLast("B"), null);
            Assert.AreEqual("".BeforeLast("B"), "");

            //BeforeLast(this string source, string match, StringComparison comparison)
            Assert.AreEqual("big big big thing".BeforeLast("big", StringComparison.Ordinal), "big big ");
            Assert.AreEqual("big big big thing".BeforeLast("B", StringComparison.Ordinal), null);
            Assert.AreEqual("big big big thing".BeforeLast("B", StringComparison.OrdinalIgnoreCase), "big big ");

            //BeforeLast(this string source, string match, bool includeMatch)
            Assert.AreEqual("big big big thing".BeforeLast("big", false), "big big ");
            Assert.AreEqual("big big big thing".BeforeLast("big", true), "big big big");
            Assert.AreEqual("big big big thing".BeforeLast("B", true), null);

            //BeforeLast(this string source, string match, bool includeMatch, StringComparison comparison)
            Assert.AreEqual("big big big thing".BeforeLast("big", false, StringComparison.Ordinal), "big big ");
            Assert.AreEqual("big big big thing".BeforeLast("big", true, StringComparison.Ordinal), "big big big");
            Assert.AreEqual("big big big thing".BeforeLast("BIG", false, StringComparison.OrdinalIgnoreCase), "big big ");
            Assert.AreEqual("big big big thing".BeforeLast("BIG", true, StringComparison.OrdinalIgnoreCase), "big big big");

        }

        [TestMethod]
        public void StringAfter()
        {
            Assert.AreEqual("I'm okay, really, really".After("really"), ", really");
            Assert.AreEqual("I'm very very very very really very happy".After("blah"), null);
            Assert.AreEqual(
                "I'm very very very very really very happy".After("blah", 0, StringComparison.OrdinalIgnoreCase), null);

            Assert.AreEqual("I'm very very very very really very happy".After("very", 0, StringComparison.Ordinal),
                            " happy");
            Assert.AreEqual(
                "I'm very very very very really very happy".After("VERY", 0, StringComparison.OrdinalIgnoreCase),
                " happy");

            Assert.AreEqual("I'm very very very very really very happy".After("very", 1, StringComparison.Ordinal),
                            " very very very really very happy");
            Assert.AreEqual(
                "I'm very very very very really very happy".After("VERY", 1, StringComparison.OrdinalIgnoreCase),
                " very very very really very happy");

            Assert.AreEqual("I'm very very very very really very happy".After("very", 2, StringComparison.Ordinal),
                            " very very really very happy");
            Assert.AreEqual(
                "I'm very very very very really very happy".After("VERY", 2, StringComparison.OrdinalIgnoreCase),
                " very very really very happy");

            Assert.AreEqual("I'm very very very very really very happy".After("very", 5, StringComparison.Ordinal),
                            " happy");
            Assert.AreEqual(
                "I'm very very very very really very happy".After("VERY", 5, StringComparison.OrdinalIgnoreCase),
                " happy");
            Assert.AreEqual(
                "I'm very very very very really very happy".After("VERY", 6, StringComparison.OrdinalIgnoreCase), null);

            Assert.AreEqual("I'm very very very very really very happy".After("very", 0), " happy");
            Assert.AreEqual("I'm very very very very really very happy".After("very", 1),
                            " very very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("very", 2), " very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("very", 5), " happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 5), null);

            Assert.AreEqual("      after 6 spaces".After("  ", 2, StringComparison.OrdinalIgnoreCase),
                            "  after 6 spaces");
            Assert.AreEqual("      after 6 spaces".After("  ", 12, StringComparison.OrdinalIgnoreCase), null);
            Assert.AreEqual("findme".After("", 2, StringComparison.OrdinalIgnoreCase), "findme");

            //with match prepended
            Assert.AreEqual("I'm okay, really, really".After("really", true), "really, really");
            Assert.AreEqual("I'm okay, really, really".After("really", 1, true), "really, really");
            Assert.AreEqual("I'm okay, really, really".After("really", 2, true), "really");
            Assert.AreEqual("I'm okay, really, really".After("REALLY", true, StringComparison.OrdinalIgnoreCase),
                            "really, really");

            Assert.AreEqual("big big big thing".After("big", 2, true, StringComparison.OrdinalIgnoreCase),
                            "big big thing");
            Assert.AreEqual("big big big thing".After("big", 0, true, StringComparison.OrdinalIgnoreCase), "big thing");
            Assert.AreEqual("big big big thing".After("big", 1, true, StringComparison.OrdinalIgnoreCase),
                            "big big big thing");
            Assert.AreEqual("      after 6 spaces".After("  ", 2, true, StringComparison.OrdinalIgnoreCase),
                            "    after 6 spaces");
            Assert.AreEqual("      after 6 spaces".After("  ", 12, true, StringComparison.OrdinalIgnoreCase), null);
            Assert.AreEqual("findme".After("", 2, true, StringComparison.OrdinalIgnoreCase), "findme");
        }


        [TestMethod]
        public void StringAfterLast()
        {


        //afterlast
            Assert.AreEqual("me me me you".AfterLast("me"), " you");
            Assert.AreEqual("me me Me you".AfterLast("me"), " Me you");
            Assert.AreEqual("me me Me you".AfterLast("me", StringComparison.OrdinalIgnoreCase), " you");
            Assert.AreEqual("me me me you".AfterLast("me", true), "me you");
            Assert.AreEqual("me me me you".AfterLast("mE", true), null);
            Assert.AreEqual("me me me you".AfterLast("ME", true, StringComparison.OrdinalIgnoreCase), "me you");
            Assert.AreEqual("me me me you".AfterLast("ME", true, StringComparison.Ordinal), null);
        }
       
        [TestMethod]
        public void StringTruncate()
        {
            Assert.AreEqual("What are you doing?".Truncate(5, StringTruncating.EllipsisCharacter), "What ...");
            Assert.AreEqual("What are you doing?".Truncate(5, StringTruncating.EllipsisWord), "What...");
            Assert.AreEqual("What are you doing?".Truncate(8, StringTruncating.EllipsisWord), "What are...");
            Assert.AreEqual("What are you doing?".Truncate(7, StringTruncating.EllipsisCenter), "What...ng?");
            Assert.AreEqual("What are you doing?".Truncate(7, StringTruncating.Word), "What are");
            Assert.AreEqual("What are you doing?".Truncate(7, StringTruncating.Character), "What ar");

        }

        [TestMethod]
        public void StringRepeat()
        {
            Assert.AreEqual(".".Repeat(), "..");
            Assert.AreEqual(".".Repeat(2), "...");

        }

        [TestMethod]
        public void StringStartWith()
        {
            //overload 1
            //edge cases
            Assert.AreEqual("fo".StartWith(""), "fo");
            Assert.AreEqual("fo".StartWith(null), "fo");
            Assert.AreEqual("fo".StartWith("fo"), "fo");
            Assert.AreEqual("".StartWith(null), "");
            Assert.AreEqual("".StartWith(""), "");
            Assert.AreEqual(((string)null).StartWith(""), "");
            Assert.AreEqual(((string)null).StartWith(null), null);
            Assert.AreEqual("fo".StartWith("f"), "fo");
            Assert.AreEqual("oph".StartWith("oo"), "ooph");
            Assert.AreEqual("oph".StartWith("opop"), "opoph");
            Assert.AreEqual("Oph".StartWith("oOo"), "oOoOph");
            Assert.AreEqual("The Kinks".StartWith("the"), "theThe Kinks");
            Assert.AreEqual("The Kinks".StartWith(" The"), " The Kinks");

            //overload 2
            Assert.AreEqual("The Kinks".StartWith("the", StringComparison.Ordinal), "theThe Kinks");
            Assert.AreEqual("The Kinks".StartWith("the", StringComparison.OrdinalIgnoreCase), "The Kinks");
            Assert.AreEqual("ooO".StartWith("oO", StringComparison.Ordinal), "oOooO");
            Assert.AreEqual("ooO".StartWith("oO", StringComparison.OrdinalIgnoreCase), "ooO");
            Assert.AreEqual("OOO".StartWith("oo", StringComparison.Ordinal), "ooOOO");
            Assert.AreEqual("OOO".StartWith("oo", StringComparison.OrdinalIgnoreCase), "OOO");

            //overload 3
            Assert.AreEqual("he Kinks".StartWith("The", true, StringComparison.Ordinal), "The Kinks");
            Assert.AreEqual("he Kinks".StartWith("THe", true, StringComparison.OrdinalIgnoreCase), "The Kinks");
            Assert.AreEqual("he Kinks".StartWith("THe", true, StringComparison.Ordinal), "THehe Kinks");
            Assert.AreEqual("he Kinks".StartWith("The", false, StringComparison.Ordinal), "Thehe Kinks");
            Assert.AreEqual("ooO".StartWith("ooo", false, StringComparison.Ordinal), "oooooO");
            Assert.AreEqual("ooO".StartWith("ooo", false, StringComparison.OrdinalIgnoreCase), "ooO");
        }

        [TestMethod]
        public void StringStartWithout()
        {
            Assert.AreEqual("".StartWithout(null), "");
            Assert.AreEqual("".StartWithout(""), "");
            Assert.AreEqual("".StartWithout("A"), "");
            Assert.AreEqual(((string)null).StartWithout("A"), null);
            Assert.AreEqual(((string)null).StartWithout(""), null);
            Assert.AreEqual(((string)null).StartWithout(null), null);

            Assert.AreEqual("AAAB".StartWithout("A"), "B");
            Assert.AreEqual("AAAB".StartWithout("a"), "AAAB");
            Assert.AreEqual("AAAB".StartWithout("a", StringComparison.OrdinalIgnoreCase), "B");
            Assert.AreEqual("AAAB".StartWithout(null), "AAAB");
            Assert.AreEqual("AAAB".StartWithout(""), "AAAB");
            Assert.AreEqual("AAAB".StartWithout("B"), "AAAB");

            Assert.AreEqual("baab".StartWithout("ba"), "ab");
            Assert.AreEqual("baab".StartWithout("ba").StartWithout("a"), "b");
        }

        [TestMethod]
        public void StringEndWith()
        {
            //overload 1
            //edge cases
            Assert.AreEqual("fo".EndWith(""), "fo");
            Assert.AreEqual("fo".EndWith("fo"), "fo");
            Assert.AreEqual("fo".EndWith(null), "fo");
            Assert.AreEqual("".EndWith(null), "");
            Assert.AreEqual("".EndWith(""), "");
            Assert.AreEqual(((string)null).EndWith(null), null);
            Assert.AreEqual(((string)null).EndWith(""), "");
            //normal cases
            Assert.AreEqual("fo".EndWith("o"), "fo");
            Assert.AreEqual("fo".EndWith("oo"), "foo");
            Assert.AreEqual("fo".EndWith("ob"), "fob");
            Assert.AreEqual("fo".EndWith("Oo"), "foOo");
            Assert.AreEqual("fo".EndWith("OO"), "foOO");
            Assert.AreEqual("foobar".EndWith("bar"), "foobar");
            Assert.AreEqual("foobar".EndWith("bars"), "foobars");
            Assert.AreEqual("foobar".EndWith("argarg"), "foobargarg");

            //overload 2
            Assert.AreEqual("fo".EndWith("o", StringComparison.Ordinal), "fo");//sanity check
            Assert.AreEqual("fo".EndWith("o", StringComparison.OrdinalIgnoreCase), "fo");
            Assert.AreEqual("fO".EndWith("o", StringComparison.OrdinalIgnoreCase), "fO");
            Assert.AreEqual("fo".EndWith("O", StringComparison.OrdinalIgnoreCase), "fo");
            Assert.AreEqual("fo".EndWith("OO", StringComparison.OrdinalIgnoreCase), "foO");
            Assert.AreEqual("fo".EndWith("OO", StringComparison.Ordinal), "foOO");

            //overload 3
            Assert.AreEqual("fo".EndWith("o", true, StringComparison.Ordinal), "fo");
            Assert.AreEqual("fo".EndWith("o", false, StringComparison.Ordinal), "fo");
            Assert.AreEqual("fO".EndWith("o", true, StringComparison.OrdinalIgnoreCase), "fO");
            Assert.AreEqual("fO".EndWith("o", false, StringComparison.OrdinalIgnoreCase), "fO");
            Assert.AreEqual("foO".EndWith("oo", true, StringComparison.OrdinalIgnoreCase), "foO");
            Assert.AreEqual("foO".EndWith("oo", false, StringComparison.OrdinalIgnoreCase), "foO");
            Assert.AreEqual("foO".EndWith("oo", false, StringComparison.Ordinal), "foOoo");
            Assert.AreEqual("fO".EndWith("oo", false, StringComparison.OrdinalIgnoreCase), "fOoo");
            Assert.AreEqual("fOO".EndWith("oo", false, StringComparison.OrdinalIgnoreCase), "fOO");
            Assert.AreEqual("fOO".EndWith("oo", false, StringComparison.Ordinal), "fOOoo");
        }


        [TestMethod]
        public void StringJoin()
        {
            Assert.AreEqual(new[] { "a", "s", "d", "f" }.Join(), "asdf");
            Assert.AreEqual(new[] { "a", "s", "d", "f" }.Join(" "), "a s d f");

        }

        
        [TestMethod]
        public void StringIndexOfOccurance()
        {
            Assert.AreEqual("hi".IndexOfOccurance("o", 1, StringComparison.Ordinal), -1);
            Assert.AreEqual("hi".IndexOfOccurance(null, 1, StringComparison.Ordinal), -1);
            Assert.AreEqual("hi".IndexOfOccurance("h", 1, StringComparison.Ordinal), 0);
            Assert.AreEqual("hi".IndexOfOccurance("i", 1, StringComparison.Ordinal), 1);
            Assert.AreEqual("hi".IndexOfOccurance("hi", 1, StringComparison.Ordinal), 0);
            Assert.AreEqual("hi".IndexOfOccurance("i", 0, StringComparison.Ordinal), 1);
            Assert.AreEqual("hi".IndexOfOccurance("i", 2, StringComparison.Ordinal), -1);
            Assert.AreEqual("hi hi".IndexOfOccurance("hi", 2, StringComparison.Ordinal), 3);
            Assert.AreEqual("hi hi hi".IndexOfOccurance("hi", 2, StringComparison.Ordinal), 3);
            Assert.AreEqual("hi hi hi".IndexOfOccurance("hi", 2, StringComparison.Ordinal), 3);
            Assert.AreEqual("hi hihi".IndexOfOccurance("hi", 3, StringComparison.Ordinal), 5);
            Assert.AreEqual("hi hihi".IndexOfOccurance("hi", 0, StringComparison.Ordinal), 5);
            Assert.AreEqual("hi hihi".IndexOfOccurance("hi", -10, StringComparison.Ordinal), 5);

            Assert.AreEqual("00 11 22 11 00 00 11 22 11 00".IndexOfOccurance("00", 2, StringComparison.Ordinal), 12);
            Assert.AreEqual("00 11 22 11 0 00 11 22 11 00".IndexOfOccurance("00", 2, StringComparison.Ordinal), 14);
        }
    }
}
