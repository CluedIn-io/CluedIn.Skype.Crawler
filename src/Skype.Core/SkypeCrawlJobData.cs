using CluedIn.Core.Crawling;

namespace CluedIn.Crawling.Skype.Core
{
    public class SkypeCrawlJobData : CrawlJobData
    {
        public string ApiKey { get; set; }
        public string email { get; set; }
    }
}
