using System;
using System.Collections.Generic;
using CluedIn.Core;
using CluedIn.Core.Agent.Jobs;
using Microsoft.Exchange.WebServices.Data;

namespace CluedIn.Crawling.Skype.Core.Models
{
    public class AttachmentModel
    {
        [NotNull]
        public Attachment Attachment { get; }

        public Item ParentItem { get; }

        [NotNull]
        public ExchangeService Service { get; }

        public AttachmentModel([NotNull] Attachment attachment, Item parentItem, [NotNull] ExchangeService service)
        {
            this.Attachment = attachment ?? throw new ArgumentNullException(nameof(attachment));
            this.ParentItem = parentItem;
            this.Service = service;
        }

        public static IEnumerable<AttachmentModel> FilterAttachments(
            IEnumerable<AttachmentModel> attachments,
            AgentJobProcessorState<SkypeCrawlJobData> state)
        {
            var skippedFirstContactPhoto = false;

            foreach (var model in attachments)
            {
                var attachment = model.Attachment;

                if (attachment is FileAttachment fileAttachment && fileAttachment.IsContactPhoto && !skippedFirstContactPhoto)
                {
                    skippedFirstContactPhoto = true;
                    continue;
                }

                if (AttachmentHelper.IsFiltered(state, attachment))
                    continue;

                if (!(attachment is FileAttachment))
                    continue;

                yield return model;
            }
        }
    }
}
