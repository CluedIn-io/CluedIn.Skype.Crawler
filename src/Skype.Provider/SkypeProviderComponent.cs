using Castle.MicroKernel.Registration;

using CluedIn.Core;
using CluedIn.Core.Providers;
// 
using CluedIn.Crawling.Skype.Core;
using CluedIn.Crawling.Skype.Infrastructure.Installers;
// 
using CluedIn.Server;
using ComponentHost;

namespace CluedIn.Provider.Skype
{
    [Component(SkypeConstants.ProviderName, "Providers", ComponentType.Service, ServerComponents.ProviderWebApi, Components.Server, Components.DataStores, Isolation = ComponentIsolation.NotIsolated)]
    public sealed class SkypeProviderComponent : ServiceApplicationComponent<EmbeddedServer>
    {
        public SkypeProviderComponent(ComponentInfo componentInfo)
            : base(componentInfo)
        {
            // Dev. Note: Potential for compiler warning here ... CA2214: Do not call overridable methods in constructors
            //   this class has been sealed to prevent the CA2214 waring being raised by the compiler
            Container.Register(Component.For<SkypeProviderComponent>().Instance(this));  
        }

        public override void Start()
        {
            Container.Install(new InstallComponents());

            Container.Register(Types.FromThisAssembly().BasedOn<IProvider>().WithServiceFromInterface().If(t => !t.IsAbstract).LifestyleSingleton());
            Container.Register(Types.FromThisAssembly().BasedOn<IEntityActionBuilder>().WithServiceFromInterface().If(t => !t.IsAbstract).LifestyleSingleton());



            State = ServiceState.Started;
        }

        public override void Stop()
        {
            if (State == ServiceState.Stopped)
                return;

            State = ServiceState.Stopped;
        }
    }
}
