using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kallithea_Klone.Other_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Machine.Fakes;
using Machine.Specifications;

namespace Kallithea_Klone.Other_Classes.Tests
{
    [TestClass()]
    public class URIPairTests : WithSubject<URIPair>
    {
        private URIPair target;

        [TestInitialize]
        public void SetUp()
        {
            target = null;
        }

        [TestMethod()]
        public void URIPairTestSimpleRemote()
        {
            URIPair target = new URIPair("", "https://github.com");
            Assert.AreEqual("https://@github.com/", target.Remote);
        }

        [TestMethod()]
        public void URIPairTestSimpleRemoteWithUsername()
        {
            URIPair target = new URIPair("", "https://github.com");
            Assert.AreEqual("https://username@github.com/", target.Remote);
        }
    }
}