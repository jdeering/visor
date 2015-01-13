using Ninject.Modules;
using Visor.Lib;

namespace Visor
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ICryptoService>().To<EmbeddedCryptoService>()
                .InSingletonScope()
                .WithPropertyValue("KeyFile", Constants.KeyFilePath)
                .WithPropertyValue("IvFile", Constants.IVFilePath);
        }
    }
}