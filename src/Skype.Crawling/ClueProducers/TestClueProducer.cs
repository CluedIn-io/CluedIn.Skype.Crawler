using System;
using CluedIn.Core.Data;
using CluedIn.Crawling.Factories;
using CluedIn.Crawling.Skype.Core.Models;

namespace CluedIn.Crawling.Skype.ClueProducers
{
    public class TestClueProducer : BaseClueProducer<TestObject>
    {
        private readonly IClueFactory _factory;

        public TestClueProducer(IClueFactory factory)
        {
            _factory = factory;
        }

        protected override Clue MakeClueImpl(TestObject input, Guid accountId)
        {
            throw new NotImplementedException();
        }
    }
}
