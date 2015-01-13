using FluentAssertions;
using NUnit.Framework;

namespace Visor.Lib.Tests
{
    [TestFixture]
    public class EmbeddedCryptoServiceTests
    {
        private EmbeddedCryptoService _crypto;

        [SetUp]
        public void Init()
        {
            _crypto = new EmbeddedCryptoService();
        }

        [Test]
        public void Encrypt_Null_ReturnsEmptyArray()
        {
            string s = null;
            _crypto.Encrypt(s).Should().BeEmpty();
        }

        [Test]
        public void Encrypt_EmptyString_ReturnsEmptyArray()
        {
            _crypto.Encrypt("").Should().BeEmpty();
        }
    }
}
