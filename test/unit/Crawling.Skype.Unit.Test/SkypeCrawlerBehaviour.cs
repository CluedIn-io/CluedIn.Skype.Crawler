using CluedIn.Core.Crawling;
using CluedIn.Crawling;
using CluedIn.Crawling.Skype;
using CluedIn.Crawling.Skype.Infrastructure.Factories;
using Moq;
using Shouldly;
using Xunit;

namespace Crawling.Skype.Unit.Test
{
    public class SkypeCrawlerBehaviour
    {
        private readonly ICrawlerDataGenerator _sut;

        public SkypeCrawlerBehaviour()
        {
            var nameClientFactory = new Mock<ISkypeClientFactory>();

            _sut = new SkypeCrawler(nameClientFactory.Object);
        }

        [Fact]
        public void GetDataReturnsData()
        {
            var jobData = new CrawlJobData();

            _sut.GetData(jobData)
                .ShouldNotBeNull();
        }
    }
}
