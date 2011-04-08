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
