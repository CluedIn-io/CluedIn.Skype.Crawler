using CluedIn.Crawling.Skype.Core;

namespace CluedIn.Crawling.Skype
{
    public class SkypeCrawlerJobProcessor : GenericCrawlerTemplateJobProcessor<SkypeCrawlJobData>
    {
        public SkypeCrawlerJobProcessor(SkypeCrawlerComponent component) : base(component)
        {
        }
    }
}
