using System;
using System.Security.Cryptography;
using FluentAssertions;
using NUnit.Framework;

namespace Visor.Lib.Tests
{
    [TestFixture]
    public class CryptoToolsTests
    {
        [Test]
        public void Encrypt_Null_ReturnsEmptyArray()
        {
            CryptoTools.Encrypt(null, null, null).Should().BeEmpty();
        }

        [Test]
        public void Encrypt_EmptyString_ReturnsEmptyArray()
        {
            CryptoTools.Encrypt("", null, null).Should().BeEmpty();
        }

        [Test]
        public void Decrypt_EmptyArray_ReturnsEmptyString()
        {
            CryptoTools.Decrypt(new byte[0], null, null).Should().BeEmpty();
        }

        [Test]
        public void Encrypt_NullKey_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CryptoTools.Encrypt("Random String", null, new byte[128]);
            });
        }

        [Test]
        public void Encrypt_EmptyKey_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CryptoTools.Encrypt("Random String", new byte[0], new byte[128]);
            });
        }

        [Test]
        public void Encrypt_InvalidKeySize_ThrowsException()
        {
            Assert.Throws<CryptographicException>(() =>
            {
                CryptoTools.Encrypt("Random String", new byte[20], new byte[128]);
            });
        }

        [Test]
        public void Encrypt_NullIV_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CryptoTools.Encrypt("Random String", new byte[128], null);
            });
        }

        [Test]
        public void Encrypt_EmptyIV_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CryptoTools.Encrypt("Random String", new byte[128], new byte[0]);
            });
        }

        [Test]
        public void Encrypt_InvalidIVSize_ThrowsException()
        {
            Assert.Throws<CryptographicException>(() =>
            {
                CryptoTools.Encrypt("Random String", new byte[128], new byte[20]);
            });
        }

        [Test]
        public void Decrypt_NullKey_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                {
                    CryptoTools.Decrypt(new byte[100], null, new byte[128]);
                });
        }

        [Test]
        public void Decrypt_EmptyKey_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                {
                    CryptoTools.Decrypt(new byte[100], new byte[0], new byte[128]);
                });
        }

        [Test]
        public void Decrypt_InvalidKeySize_ThrowsException()
        {
            Assert.Throws<CryptographicException>(() =>
                {
                    CryptoTools.Decrypt(new byte[100], new byte[20], new byte[128]);
                });
        }

        [Test]
        public void Decrypt_NullIV_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                {
                    CryptoTools.Decrypt(new byte[100], new byte[128], null);
                });
        }

        [Test]
        public void Decrypt_EmptyIV_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                {
                    CryptoTools.Decrypt(new byte[100], new byte[128], new byte[0]);
                });
        }

        [Test]
        public void Decrypt_InvalidIVSize_ThrowsException()
        {
            Assert.Throws<CryptographicException>(() =>
                {
                    CryptoTools.Decrypt(new byte[100], new byte[128], new byte[20]);
                });
        }
    }
}