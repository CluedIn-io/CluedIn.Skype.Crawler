using System;
using CluedIn.Core.Agent.Jobs;
using CluedIn.Crawling.Factories;
using Microsoft.Exchange.WebServices.Data;
using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Crawling.Skype.Core;
using CluedIn.Crawling.Skype.Vocabularies;
using CluedIn.Crawling.Helpers;

namespace CluedIn.Crawling.Skype.ClueProducers
{
    public class ConversationClueProducer : BaseClueProducer<Conversation>
    {
        private readonly IClueFactory _factory;
        private readonly ApplicationContext _appContext;
        private readonly IAgentJobProcessorState<SkypeCrawlJobData> _state;

        public ConversationClueProducer(IClueFactory factory, ApplicationContext appContext)
        {
            _factory = factory;
            _appContext = appContext;

            try
            {
                _state = _appContext.Container.Resolve<IAgentJobProcessorState<SkypeCrawlJobData>>();
            }
            catch
            {
                throw new ArgumentException($"Argument {nameof(appContext)} does not contain IAgentJobProcessorState");
            }
        }

        protected override Clue MakeClueImpl(Conversation input, Guid accountId)
        {
            var clue = _factory.Create(EntityType.Mail, input.Id.UniqueId, accountId);

            var data = clue.Data.EntityData;

            if (input.Topic != null)
            {
                data.Name = input.Topic;
                data.DisplayName = input.Topic;
            }
            else
            {
                data.Name = "No Topic Set";
                data.DisplayName = "No Topic Set";
            }

            if (input.LastModifiedTime != null)
                data.ModifiedDate = input.LastModifiedTime;

            var vocab = new ConversationVocabulary();

            data.Properties[vocab.Importance] = input.Importance.ToString();
            data.Properties[vocab.Categories] = input.Categories.PrintIfAvailable();
            data.Properties[vocab.FlagStatus] = input.FlagStatus.PrintIfAvailable();
            data.Properties[vocab.GlobalCategories] = input.GlobalCategories.PrintIfAvailable();
            data.Properties[vocab.GlobalFlagStatus] = input.GlobalFlagStatus.PrintIfAvailable();
            data.Properties[vocab.GlobalHasAttachments] = input.GlobalHasAttachments.PrintIfAvailable();
            data.Properties[vocab.GlobalHasIrm] = input.GlobalHasIrm.PrintIfAvailable();
            data.Properties[vocab.GlobalImportance] = input.GlobalImportance.PrintIfAvailable();
            data.Properties[vocab.GlobalItemClasses] = input.GlobalItemClasses.PrintIfAvailable();
            data.Properties[vocab.GlobalItemIds] = JsonUtility.Serialize(input.GlobalItemIds);
            data.Properties[vocab.GlobalLastDeliveryTime] = input.GlobalLastDeliveryTime.PrintIfAvailable();
            data.Properties[vocab.GlobalMessageCount] = input.GlobalMessageCount.PrintIfAvailable();
            data.Properties[vocab.GlobalSize] = input.GlobalSize.PrintIfAvailable();
            data.Properties[vocab.GlobalUniqueRecipients] = input.GlobalUniqueRecipients.PrintIfAvailable();
            data.Properties[vocab.GlobalUniqueSenders] = input.GlobalUniqueSenders.PrintIfAvailable();
            data.Properties[vocab.GlobalUniqueUnreadSenders] = input.GlobalUniqueUnreadSenders.PrintIfAvailable();
            data.Properties[vocab.GlobalUnreadCount] = input.GlobalUnreadCount.PrintIfAvailable();
            data.Properties[vocab.HasIrm] = input.HasIrm.PrintIfAvailable();
            data.Properties[vocab.HasAttachments] = input.HasAttachments.PrintIfAvailable();
            data.Properties[vocab.InstanceKey] = input.InstanceKey.PrintIfAvailable();
            data.Properties[vocab.IsDirty] = input.IsDirty.PrintIfAvailable();
            data.Properties[vocab.IsNew] = input.IsNew.PrintIfAvailable();
            data.Properties[vocab.ItemClasses] = input.ItemClasses.PrintIfAvailable();
            data.Properties[vocab.ItemIds] = JsonUtility.Serialize(input.ItemIds);
            data.Properties[vocab.LastDeliveryTime] = input.LastDeliveryTime.PrintIfAvailable();
            data.Properties[vocab.LastModifiedTime] = input.LastModifiedTime.PrintIfAvailable();
            data.Properties[vocab.MessageCount] = input.MessageCount.PrintIfAvailable();
            data.Properties[vocab.Size] = input.Size.PrintIfAvailable();
            data.Properties[vocab.Topic] = input.Topic.PrintIfAvailable();
            data.Properties[vocab.UniqueRecipients] = input.UniqueRecipients.PrintIfAvailable();
            data.Properties[vocab.UniqueSenders] = input.UniqueSenders.PrintIfAvailable();
            data.Properties[vocab.UniqueUnreadSenders] = input.UniqueUnreadSenders.PrintIfAvailable();
            data.Properties[vocab.UnreadCount] = input.UnreadCount.PrintIfAvailable();

            return clue;
        }
    }
}
