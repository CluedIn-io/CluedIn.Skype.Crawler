using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.Crawling.Skype.Core
{
    public class SkypeCrawlJobData : CrawlJobData
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public long? FileSizeLimit { get; set; }
        public List<string> IgnoredFileTypes { get; set; }

        public SkypeCrawlJobData() { }

        public SkypeCrawlJobData(Dictionary<string, object> configuration)
        {
            Email = GetValue<string>(configuration, "email");
            Password = GetValue<string>(configuration, "password");
            FileSizeLimit = GetValue<long?>(configuration, "FileSizeLimit", fallback: 10000); // TODO: consider fallback value
            IgnoredFileTypes = GetValue<List<string>>(configuration, "IgnoredFileTypes");
        }
    }
}
