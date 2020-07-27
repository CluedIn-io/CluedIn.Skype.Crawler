using Castle.Windsor;
using CluedIn.Core;
using CluedIn.Core.Providers;
using CluedIn.Crawling.Skype.Infrastructure.Factories;
using Moq;

namespace CluedIn.Provider.Skype.Unit.Test.SkypeProvider
{
    public abstract class SkypeProviderTest
    {
        protected readonly ProviderBase Sut;

        protected Mock<ISkypeClientFactory> NameClientFactory;
        protected Mock<IWindsorContainer> Container;

        protected SkypeProviderTest()
        {
            Container = new Mock<IWindsorContainer>();
            NameClientFactory = new Mock<ISkypeClientFactory>();
            var applicationContext = new ApplicationContext(Container.Object);
            Sut = new Skype.SkypeProvider(applicationContext, NameClientFactory.Object);
        }
    }
}
