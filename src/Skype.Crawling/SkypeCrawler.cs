using System.Collections.Generic;

using CluedIn.Core.Crawling;
using CluedIn.Crawling.Skype.Core;
using CluedIn.Crawling.Skype.Core.Models;
using CluedIn.Crawling.Skype.Infrastructure.Factories;
using Microsoft.Exchange.WebServices.Data;

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

            //var client = clientFactory.CreateNew(skypecrawlJobData);

            ////retrieve data from provider and yield objects
            //foreach (var item in client.Get())
            //{
            //    var output = new Mail { EmailMessage = item as EmailMessage };
            //    yield return output;
            //}

            yield return new TestObject { IntProperty = 0, StringProperty = "Something" };
            yield return new TestObject { IntProperty = 1, StringProperty = "Something else" };
            yield return new TestObject { IntProperty = 2, StringProperty = "Something else else" };
        }
    }
}
