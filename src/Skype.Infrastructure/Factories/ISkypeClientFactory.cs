using CluedIn.Crawling.Skype.Core;

namespace CluedIn.Crawling.Skype.Infrastructure.Factories
{
    public interface ISkypeClientFactory
    {
        SkypeClient CreateNew(SkypeCrawlJobData skypeCrawlJobData);
    }
}
