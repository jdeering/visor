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

        [Test]
        public void Null_IsBlank_ReturnsTrue()
        {
            string s = null;
            s.IsBlank().Should().BeTrue();
        }

        [Test]
        public void Null_IsNotBlank_ReturnsFalse()
        {
            string s = null;
            s.IsNotBlank().Should().BeFalse();
        }

        [Test]
        public void EmptyString_IsBlank_ReturnsTrue()
        {
            string s = string.Empty;
            s.IsBlank().Should().BeTrue();
        }

        [Test]
        public void EmptyString_IsNotBlank_ReturnsFalse()
        {
            string s = string.Empty;
            s.IsNotBlank().Should().BeFalse();
        }

        [Test]
        public void Space_IsBlank_ReturnsFalse()
        {
            string s = " ";
            s.IsBlank().Should().BeFalse();
        }

        [Test]
        public void Space_IsNotBlank_ReturnsTrue()
        {
            string s = " ";
            s.IsNotBlank().Should().BeTrue();
        }
    }
}