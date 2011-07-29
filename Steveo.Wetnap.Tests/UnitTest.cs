using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Steveo.Wetnap.Tests
{
    [TestClass]
    public class UnitTest
    {

        
        [TestMethod]
        public void TestStringBefore()
        {
            Assert.AreEqual("I'm okay, really, really".Before("really"), "I'm okay, ");
        }

        [TestMethod]
        public void TestStringAfter()
        {
            Assert.AreEqual("I'm okay, really, really".After("really"), ", really");

            Assert.AreEqual("I'm very very very very really very happy".After("very", 0, false, StringComparison.Ordinal), " happy");
            Assert.AreEqual("I'm very very very very really very happy".After("very", 0, true, StringComparison.Ordinal), "very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 0, false, StringComparison.OrdinalIgnoreCase), " happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 0, true, StringComparison.OrdinalIgnoreCase), "very happy");

            Assert.AreEqual("I'm very very very very really very happy".After("very", 1, false, StringComparison.Ordinal), " very very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("very", 1, true, StringComparison.Ordinal), "very very very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 1, false, StringComparison.OrdinalIgnoreCase), " very very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 1, true, StringComparison.OrdinalIgnoreCase), "very very very very really very happy");

            Assert.AreEqual("I'm very very very very really very happy".After("very", 2, false, StringComparison.Ordinal), " very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("very", 2, true, StringComparison.Ordinal), "very very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 2, false, StringComparison.OrdinalIgnoreCase), " very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 2, true, StringComparison.OrdinalIgnoreCase), "very very very really very happy");

            Assert.AreEqual("I'm very very very very really very happy".After("very", 5, false, StringComparison.Ordinal), " happy");
            Assert.AreEqual("I'm very very very very really very happy".After("very", 5, true, StringComparison.Ordinal), "very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 5, false, StringComparison.OrdinalIgnoreCase), " happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 5, true, StringComparison.OrdinalIgnoreCase), "very happy");

            Assert.AreEqual("      after 6 spaces".After("  ", 2, false, StringComparison.OrdinalIgnoreCase), "  after 6 spaces");
            Assert.AreEqual("      after 6 spaces".After("  ", 12, false, StringComparison.OrdinalIgnoreCase), null);
            Assert.AreEqual("findme".After("", 2, true, StringComparison.OrdinalIgnoreCase), "findme");

        }

        [TestMethod]
        public void TestStringTruncate()
        {
            Assert.AreEqual("What are you doing?".Truncate(5, StringTruncating.EllipsisCharacter), "What ...");
            Assert.AreEqual("What are you doing?".Truncate(5, StringTruncating.EllipsisWord), "What...");
            Assert.AreEqual("What are you doing?".Truncate(8, StringTruncating.EllipsisWord), "What are...");
            Assert.AreEqual("What are you doing?".Truncate(7, StringTruncating.EllipsisCenter), "What...ng?");
            Assert.AreEqual("What are you doing?".Truncate(7, StringTruncating.Word), "What are");
            Assert.AreEqual("What are you doing?".Truncate(7, StringTruncating.Character), "What ar");

        }


        [TestMethod]
        public void TestHasChars()
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
        public void TestCharsOr()
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

    }
}
