using CluedIn.Core;
using CluedIn.Crawling.Skype.Core;

using ComponentHost;

namespace CluedIn.Crawling.Skype
{
    [Component(SkypeConstants.CrawlerComponentName, "Crawlers", ComponentType.Service, Components.Server, Components.ContentExtractors, Isolation = ComponentIsolation.NotIsolated)]
    public class SkypeCrawlerComponent : CrawlerComponentBase
    {
        public SkypeCrawlerComponent([NotNull] ComponentInfo componentInfo)
            : base(componentInfo)
        {
        }
    }
}

