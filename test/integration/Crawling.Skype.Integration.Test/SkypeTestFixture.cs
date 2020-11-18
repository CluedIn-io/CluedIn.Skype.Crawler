using System.IO;
using System.Reflection;
using CluedIn.Crawling.Skype.Core;
using CluedIn.Core.Agent.Jobs;
using Castle.MicroKernel.Registration;
using Xunit.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;
using CluedIn.Core.Data;
using DebugCrawlerHost = CrawlerIntegrationTesting.CrawlerHost.DebugCrawlerHost<CluedIn.Crawling.Skype.Core.SkypeCrawlJobData>;
using CrawlerIntegrationTesting.Clues;
using CrawlerIntegrationTesting.Log;

namespace CluedIn.Crawling.Skype.Integration.Test
{
    public class SkypeTestFixture
    {
        public ClueStorage ClueStorage { get; }

        private readonly DebugCrawlerHost debugCrawlerHost;

        public TestLogger Log { get; }
        public SkypeTestFixture()
        {
            var executingFolder = new FileInfo(Assembly.GetExecutingAssembly().CodeBase.Substring(8)).DirectoryName;
            debugCrawlerHost = new DebugCrawlerHost(executingFolder, SkypeConstants.ProviderName);

            ClueStorage = new ClueStorage();

            Log = debugCrawlerHost.AppContext.Container.Resolve<TestLogger>();

            debugCrawlerHost.ProcessClue += ClueStorage.AddClue;

            //var debugAgentJobProcessorState = new CrawlerIntegrationTesting.CrawlerHost.DebugAgentJobProcessorState<SkypeCrawlJobData>
            //{
            //    Log = new LoggingTargetLogger(this.Log),
            //    CancellationTokenSource = new System.Threading.CancellationTokenSource(),
            //    JobData = new SkypeCrawlJobData(SkypeConfiguration.Create()),
            //    Status = new AgentJobStatus(),
            //    TaskFactory = new TaskFactory(),
            //    Result = new AgentJobResult()
            //};

            //debugCrawlerHost.AppContext.Container.Register(Component.For<IAgentJobProcessorState<SkypeCrawlJobData>>().Instance(debugAgentJobProcessorState));

            debugCrawlerHost.Execute(SkypeConfiguration.Create(), SkypeConstants.ProviderId);
        }

        //public void AddClue(Clue clue)
        //{
        //    var entityType = clue.Data.ProcessedEntityData.EntityType;
        //    if (EntityTypeCounts.ContainsKey(entityType))
        //    {
        //        EntityTypeCounts[entityType] += 1;
        //    }
        //    else
        //    {
        //        EntityTypeCounts.Add(entityType, 1);
        //    }
        //}

        public void PrintClues(ITestOutputHelper output)
        {
            foreach (var clue in ClueStorage.Clues)
            {
                output.WriteLine(clue.OriginEntityCode.ToString());
            }
        }

        public void PrintLogs(ITestOutputHelper output)
        {
            output.WriteLine(Log.PrintLogs());
        }

        public void Dispose()
        {
        }

    }
}


