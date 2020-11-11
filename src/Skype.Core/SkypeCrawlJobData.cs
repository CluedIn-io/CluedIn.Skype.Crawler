using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.Crawling.Skype.Core
{
    public class SkypeCrawlJobData : CrawlJobData
    {
        public string email { get; set; }
        public string password { get; set; }
        public long? FileSizeLimit { get; set; }
        public List<string> IgnoredFileTypes { get; set; }
    }
}
