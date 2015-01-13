using FluentAssertions;
using NUnit.Framework;
using Ninject;
using Visor.Lib;

namespace Visor.Tests
{
    [TestFixture]
    public class ServiceModuleTests
    {
        private IKernel _ninject;

        [SetUp]
        public void Init()
        {
            _ninject = new StandardKernel(new ServiceModule());

        }

        [Test]
        public void ICryptoService_IsCorrectType()
        {
            var service = _ninject.Get<ICryptoService>();
            service.Should().BeOfType<EmbeddedCryptoService>();
        }

        [Test]
        public void ICryptoService_HasKeyFile()
        {
            var service = _ninject.Get<ICryptoService>() as EmbeddedCryptoService;
            service.KeyFile.Should().NotBeEmpty();
        }

        [Test]
        public void ICryptoService_HasIvFile()
        {
            var service = _ninject.Get<ICryptoService>() as EmbeddedCryptoService;
            service.IvFile.Should().NotBeEmpty();
        }

        [Test]
        public void ICryptoService_IsSingleton()
        {
            var service1 = _ninject.Get<ICryptoService>();
            var service2 = _ninject.Get<ICryptoService>();

            service1.Should().BeSameAs(service2);
        }
    }
}
