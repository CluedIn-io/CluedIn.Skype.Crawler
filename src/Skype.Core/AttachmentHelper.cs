using System.Collections.Generic;
using System.IO;
using System.Linq;
using CluedIn.Core.Agent.Jobs;
using CluedIn.Core.IO;
using Microsoft.Exchange.WebServices.Data;

namespace CluedIn.Crawling.Skype.Core
{
    public static class AttachmentHelper
    {

        public static bool IsFiltered(IAgentJobProcessorState<SkypeCrawlJobData> state, Attachment value)
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
