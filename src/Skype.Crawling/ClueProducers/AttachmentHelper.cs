using System.Collections.Generic;
using System.IO;
using System.Linq;
using CluedIn.Core.Agent.Jobs;
using CluedIn.Core.IO;
using CluedIn.Crawling.Skype.Core;
using Microsoft.Exchange.WebServices.Data;

namespace CluedIn.Crawling.Skype.ClueProducers
{
    public static class AttachmentHelper
    {
        public static IEnumerable<Attachment> Filter(this IEnumerable<Attachment> items, AgentJobProcessorState<SkypeCrawlJobData> state)
        {
            return items.Where(a => !IsFiltered(state, a));
        }

        public static bool IsFiltered(AgentJobProcessorState<SkypeCrawlJobData> state, Attachment value)
        {
            var cleanName = PathEx.GetCleanFileName(value.Name);
            var fileNameExtension = Path.GetExtension(cleanName);

            if (state.JobData.FileSizeLimit != null && value.Size > state.JobData.FileSizeLimit.Value)
                return true;

            if (!string.IsNullOrEmpty(fileNameExtension) && state.JobData.IgnoredFileTypes != null && state.JobData.IgnoredFileTypes.Contains(fileNameExtension.ToLowerInvariant()))
                return true;

            return false;
        }
    }
}
