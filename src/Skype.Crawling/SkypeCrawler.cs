using System.Collections.Generic;

using CluedIn.Core.Crawling;
using CluedIn.Crawling.Skype.Core;
using CluedIn.Crawling.Skype.Infrastructure.Factories;

namespace CluedIn.Crawling.Skype
{
    public class SkypeCrawler : ICrawlerDataGenerator
    {
        private readonly ISkypeClientFactory clientFactory;
        public SkypeCrawler(ISkypeClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public IEnumerable<object> GetData(CrawlJobData jobData)
        {
            if (!(jobData is SkypeCrawlJobData skypecrawlJobData))
            {
                yield break;
            }

            var client = clientFactory.CreateNew(skypecrawlJobData);

            //retrieve data from provider and yield objects
            
        }       
    }
}
