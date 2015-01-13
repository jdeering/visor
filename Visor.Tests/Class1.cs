using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Visor.Extensions;

namespace Visor.Tests
{
    [TestFixture]
    public class CryptoToolsTests
    {
        [Test]
        public void Encrypt_NullString()
        {
            string s = null;
            CryptoTools.Encrypt(s).Should().BeEmpty();
        }

        [Test]
        public void Encrypt_EmptyString()
        {
            var s = "";
            CryptoTools.Encrypt(s).Should().BeEmpty();
        }
    }


}
