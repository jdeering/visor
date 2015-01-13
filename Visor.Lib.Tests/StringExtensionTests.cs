using FluentAssertions;
using NUnit.Framework;

namespace Visor.Lib.Tests
{
    [TestFixture]
    public class StringExtensionTests
    {
        [Test]
        public void Encrypt_Null_ReturnsEmptyArray()
        {
            string s = null;
            s.Encrypt().Should().BeEmpty();
        }

        [Test]
        public void Encrypt_EmptyString_ReturnsEmptyArray()
        {
            "".Encrypt().Should().BeEmpty();
        }

        [Test]
        public void Decrypt_EmptyArray_ReturnsEmptyString()
        {
            (new byte[0]).Decrypt().Should().BeEmpty();
        }
    }
}