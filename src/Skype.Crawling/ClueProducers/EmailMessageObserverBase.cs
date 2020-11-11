using System;
using System.Linq;

using CluedIn.Core;
using CluedIn.Core.Agent.Jobs;
using CluedIn.Core.Data;
using CluedIn.Crawling.ExchangeSharedMailbox.Factories;
using CluedIn.Crawling.ExchangeSharedMailbox.Helpers;
using CluedIn.Crawling.ExchangeSharedMailbox.Models;
using CluedIn.Crawling.ExchangeSharedMailbox.Vocabularies;

using Microsoft.Exchange.WebServices.Data;

namespace CluedIn.Crawling.ExchangeSharedMailbox.Subjects
{
    public class EmailMessageObserverBase : ItemObserverBase
    {
        [NotNull]
        private readonly AgentJobProcessorState<ExchangeSharedMailboxCrawlJobData> state;

        protected EmailMessageObserverBase([NotNull] ApplicationContext appContext,
                                      [NotNull] AgentJobProcessorState<ExchangeSharedMailboxCrawlJobData> state,
                                      [NotNull] IClueFactory factory)
            : base(appContext, state, factory)
        {
            this.state = state;
        }

        protected void PopulateEmailMessage(Clue clue, IEmailMessageModelBase<EmailMessage> message, ExchangeSharedMailboxMailBaseVocabulary vocabulary, ExchangeService service)
        {
            var value = message.Object;
            var data = clue.Data.EntityData;

            this.PopulateItem(clue, message, vocabulary, service);

            data.Name = value.ExPrintIfAvailable(v => v.Subject);
            data.CreatedDate = value.ExGetIfAvailable(v => (DateTimeOffset?)v.DateTimeReceived, null)
                            ?? value.ExGetIfAvailable(v => (DateTimeOffset?)v.DateTimeSent, null)
                            ?? value.ExGetIfAvailable(v => (DateTimeOffset?)v.DateTimeCreated, null);

            if (!string.IsNullOrEmpty(value.InternetMessageId))
                data.Codes.Add(new EntityCode(data.EntityType, CodeOrigin.CluedIn.CreateSpecific("InternetMessageId"), value.InternetMessageId));

            // TODO: Get OWA address
            //data.Uri = new Uri(service.Url, "/owa/" + value.WebClientReadFormQueryString);

            data.Properties[vocabulary.DateTimeReceived] = value.ExPrintIfAvailable(v => v.DateTimeReceived);
            data.Properties[vocabulary.DateTimeSent] = value.ExPrintIfAvailable(v => v.DateTimeSent);
            data.Properties[vocabulary.IsRead] = value.ExPrintIfAvailable(v => v.IsRead);
            data.Properties[vocabulary.InternetMessageId] = value.ExPrintIfAvailable(v => v.InternetMessageId);
            data.Properties[vocabulary.ConversationTopic] = value.ExPrintIfAvailable(v => v.ConversationTopic);
            data.Properties[vocabulary.IsAssociated] = value.ExPrintIfAvailable(v => v.IsAssociated);
            data.Properties[vocabulary.IsDeliveryReceiptRequested] = value.ExPrintIfAvailable(v => v.IsDeliveryReceiptRequested);
            data.Properties[vocabulary.IsResponseRequested] = value.ExPrintIfAvailable(v => v.IsResponseRequested);
            data.Properties[vocabulary.References] = value.ExPrintIfAvailable(v => v.References);

            this.WriteAddressProperty(data, vocabulary.ReceivedBy, value.ExGetIfAvailable(v => v.ReceivedBy, null));
            this.WriteAddressProperty(data, vocabulary.ReceivedRepresenting, value.ExGetIfAvailable(v => v.ReceivedRepresenting, null));

            if (value.ReplyTo != null)
            {
                if (value.ReplyTo.Count == 1)
                    this.WriteAddressProperty(data, vocabulary.ReplyTo, value.ReplyTo.First());
                else
                    data.Properties[vocabulary.ReplyTo] = string.Join(";", value.ReplyTo.Where(a => a.Address != null && MailAddressUtility.IsValid(a.Address)).Select(a => a.Address));
            }

            PersonReference fromReference = null;

            // From
            if (value.From != null)
                fromReference = this.AddCreatedBy(clue, value.From, vocabulary.From);

            // Sender
            if (value.Sender != null)
            {
                this.AddCreatedBy(clue, value.Sender, vocabulary.Sender);
                data.LastChangedBy = fromReference ?? data.LastChangedBy;
            }

            // Recipients
            {
                var tmp = new PropertySet(BasePropertySet.FirstClassProperties, EmailMessageSchema.ToRecipients, EmailMessageSchema.BccRecipients, EmailMessageSchema.CcRecipients, ItemSchema.Attachments);
                var iv = new ItemView(1000)
                {
                    PropertySet = tmp
                };

                value.ExLoad(this.state, tmp);

                this.AddEmailAddressEdges(clue, value.BccRecipients, EntityEdgeType.Recipient);
                this.AddEmailAddressEdges(clue, value.CcRecipients, EntityEdgeType.Recipient);
                this.AddEmailAddressEdges(clue, value.ToRecipients, EntityEdgeType.Recipient);
            }
        }
    }
}
