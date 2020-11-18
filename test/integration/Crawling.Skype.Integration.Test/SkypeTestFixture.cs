using System.IO;
using System.Reflection;
using Castle.MicroKernel.Registration;
using CluedIn.Crawling.Skype.Core;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using DebugCrawlerHost = CrawlerIntegrationTesting.CrawlerHost.DebugCrawlerHost<CluedIn.Crawling.Skype.Core.SkypeCrawlJobData>;
using System.Collections.Generic;
using CluedIn.Core.Data;
using CluedIn.Core.Agent.Jobs;
using System.Threading.Tasks;

namespace CluedIn.Crawling.Skype.Integration.Test
{
    public class SkypeTestFixture
    {
        //public ClueStorage ClueStorage { get; }
        public Dictionary<string, int> EntityTypeCounts;

        private readonly DebugCrawlerHost debugCrawlerHost;

        public ILogger<SkypeTestFixture> Log { get; }

        public SkypeTestFixture()
        {
            var executingFolder = new FileInfo(Assembly.GetExecutingAssembly().CodeBase.Substring(8)).DirectoryName;
            debugCrawlerHost = new DebugCrawlerHost(executingFolder, SkypeConstants.ProviderName, c => {
                c.Register(Component.For<ILogger>().UsingFactoryMethod(_ => NullLogger.Instance).LifestyleSingleton());
                c.Register(Component.For<ILoggerFactory>().UsingFactoryMethod(_ => NullLoggerFactory.Instance).LifestyleSingleton());
            });

            //ClueStorage = new ClueStorage();
            EntityTypeCounts = new Dictionary<string, int>();

            Log = debugCrawlerHost.AppContext.Container.Resolve<ILogger<SkypeTestFixture>>();

            debugCrawlerHost.ProcessClue += IncrementEntityTypeCount;

            var debugAgentJobProcessorState = new CrawlerIntegrationTesting.CrawlerHost.DebugAgentJobProcessorState<SkypeCrawlJobData>
            {
                Log = Log,
                CancellationTokenSource = new System.Threading.CancellationTokenSource(),
                JobData = new SkypeCrawlJobData(SkypeConfiguration.Create()),
                Status = new AgentJobStatus(),
                TaskFactory = new TaskFactory(),
                Result = new AgentJobResult()
            };

            debugCrawlerHost.AppContext.Container.Register(Component.For<IAgentJobProcessorState<SkypeCrawlJobData>>().Instance(debugAgentJobProcessorState));

            debugCrawlerHost.Execute(SkypeConfiguration.Create(), SkypeConstants.ProviderId);
        }

        public void IncrementEntityTypeCount(Clue clue)
        {
            var entityType = clue.Data.EntityData.EntityType;
            if (EntityTypeCounts.ContainsKey(entityType))
            {
                EntityTypeCounts[entityType]++;
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
                output.WriteLine($"{entityType} count: {EntityTypeCounts[entityType]}");
            }
        }

        public void PrintLogs(ITestOutputHelper output)
        {
            //TODO:
            //output.WriteLine(Log.PrintLogs());
        }

        public void Dispose()
        {
        }

    }
}


