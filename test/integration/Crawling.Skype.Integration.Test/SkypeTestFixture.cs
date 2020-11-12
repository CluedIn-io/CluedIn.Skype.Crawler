using System.IO;
using System.Reflection;
using CluedIn.Crawling.Skype.Core;
using CluedIn.Core.Agent.Jobs;
using Castle.MicroKernel.Registration;
using CrawlerIntegrationTesting.Log;
using CluedIn.Core.Logging;
using Xunit.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;
using CluedIn.Core.Data;
using DebugCrawlerHost = CrawlerIntegrationTesting.CrawlerHost.DebugCrawlerHost<CluedIn.Crawling.Skype.Core.SkypeCrawlJobData>;

namespace CluedIn.Crawling.Skype.Integration.Test
{
    public class SkypeTestFixture
    {
        public Dictionary<string, int> EntityTypeCounts { get; } 
        private readonly DebugCrawlerHost debugCrawlerHost;

        public TestLogger Log { get; }
        public SkypeTestFixture()
        {
            var executingFolder = new FileInfo(Assembly.GetExecutingAssembly().CodeBase.Substring(8)).DirectoryName;
            debugCrawlerHost = new DebugCrawlerHost(executingFolder, SkypeConstants.ProviderName);

            EntityTypeCounts = new Dictionary<string, int>();

            Log = debugCrawlerHost.AppContext.Container.Resolve<TestLogger>();

            debugCrawlerHost.ProcessClue += this.AddClue;

            var debugAgentJobProcessorState = new CrawlerIntegrationTesting.CrawlerHost.DebugAgentJobProcessorState<SkypeCrawlJobData>
            {
                Log = new LoggingTargetLogger(this.Log),
                CancellationTokenSource = new System.Threading.CancellationTokenSource(),
                JobData = new SkypeCrawlJobData(SkypeConfiguration.Create()),
                Status = new AgentJobStatus(),
                TaskFactory = new TaskFactory(),
                Result = new AgentJobResult()
            };

            debugCrawlerHost.AppContext.Container.Register(Component.For<IAgentJobProcessorState<SkypeCrawlJobData>>().Instance(debugAgentJobProcessorState));

            debugCrawlerHost.Execute(SkypeConfiguration.Create(), SkypeConstants.ProviderId);
        }

        public void AddClue(Clue clue)
        {
            var entityType = clue.Data.ProcessedEntityData.EntityType;
            if (EntityTypeCounts.ContainsKey(entityType))
            {
                EntityTypeCounts[entityType] += 1;
            }
            else
            {
                EntityTypeCounts.Add(entityType, 1);
            }
        }

        public void PrintClues(ITestOutputHelper output)
        {
            foreach(var entityType in EntityTypeCounts.Keys)
            {
                output.WriteLine($"{entityType}: {EntityTypeCounts[entityType]}");
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


