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
        public void TestHasChars()
        {
            Assert.IsTrue(!"".HasChars());
            Assert.IsTrue(" ".HasChars());
            Assert.IsTrue(" ".HasChars(true));
            Assert.IsTrue(!" ".HasChars(false));
        }

        
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
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 0, true, StringComparison.OrdinalIgnoreCase), "VERY happy");

            Assert.AreEqual("I'm very very very very really very happy".After("very", 1, false, StringComparison.Ordinal), " very very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("very", 1, true, StringComparison.Ordinal), "very very very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 1, false, StringComparison.OrdinalIgnoreCase), " very very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 1, true, StringComparison.OrdinalIgnoreCase), "VERY very very very really very happy");

            Assert.AreEqual("I'm very very very very really very happy".After("very", 2, false, StringComparison.Ordinal), " very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("very", 2, true, StringComparison.Ordinal), "very very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 2, false, StringComparison.OrdinalIgnoreCase), " very very really very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 2, true, StringComparison.OrdinalIgnoreCase), "VERY very very really very happy");

            Assert.AreEqual("I'm very very very very really very happy".After("very", 5, false, StringComparison.Ordinal), " happy");
            Assert.AreEqual("I'm very very very very really very happy".After("very", 5, true, StringComparison.Ordinal), "very happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 5, false, StringComparison.OrdinalIgnoreCase), " happy");
            Assert.AreEqual("I'm very very very very really very happy".After("VERY", 5, true, StringComparison.OrdinalIgnoreCase), "VERY happy");

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

    }
}
