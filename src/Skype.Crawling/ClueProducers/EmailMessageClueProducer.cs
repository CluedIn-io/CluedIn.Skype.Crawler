using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CluedIn.Core;
using CluedIn.Core.Agent.Jobs;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Data.Vocabularies;
using CluedIn.Core.IO;
using CluedIn.Crawling.Factories;
using CluedIn.Crawling.Helpers;
using CluedIn.Crawling.Skype.Core;
using CluedIn.Crawling.Skype.Core.Models;
using CluedIn.Crawling.Skype.Vocabularies;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Logging;

namespace CluedIn.Crawling.Skype.ClueProducers
{
    public class EmailMessageClueProducer : BaseClueProducer<EmailMessage>
    {
        private readonly IClueFactory _factory;
        private readonly ApplicationContext _appContext;
        private readonly IAgentJobProcessorState<SkypeCrawlJobData> _state;

        public EmailMessageClueProducer(IClueFactory factory, ApplicationContext appContext)
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

        protected override Clue MakeClueImpl(EmailMessage input, Guid accountId)
        {
            var clue = _factory.Create(EntityType.Mail, input.Id.UniqueId, accountId);

            var data = clue.Data.EntityData;

            input = EmailMessage.Bind(input.Service, input.Id).Result;

            // Metadata

            if (!string.IsNullOrWhiteSpace(input.Subject))
                data.Name = input.Subject.PrintIfAvailable();

            data.CreatedDate = (DateTimeOffset?)input.DateTimeReceived
                            ?? (DateTimeOffset?)input.DateTimeSent
                            ?? (DateTimeOffset?)input.DateTimeCreated;

            data.ModifiedDate = (DateTime?)input.LastModifiedTime
                               ?? (DateTime?)input.DateTimeSent;

            if (!string.IsNullOrWhiteSpace(input.Culture))
                data.Culture = new CultureInfo(input.Culture);

            // Entity codes

            if (!string.IsNullOrEmpty(input.InternetMessageId))
                data.Codes.Add(new EntityCode(data.EntityType, CodeOrigin.CluedIn.CreateSpecific("InternetMessageId"), input.InternetMessageId));

            // EmailMessage vocabulary
            var vocabulary = new MailVocabulary();
            if (input.TryGetProperty(ItemSchema.DateTimeReceived, out var dateTimeReceived))
                data.Properties[vocabulary.DateTimeReceived] = input.DateTimeReceived.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.DateTimeSent, out var dateTimeSent))
                data.Properties[vocabulary.DateTimeSent] = input.DateTimeSent.PrintIfAvailable();
            if (input.TryGetProperty(EmailMessageSchema.IsRead, out var isRead))
                data.Properties[vocabulary.IsRead] = input.IsRead.PrintIfAvailable();
            if (input.TryGetProperty(EmailMessageSchema.InternetMessageId, out var internetMessageId))
                data.Properties[vocabulary.InternetMessageId] = input.InternetMessageId.PrintIfAvailable();
            if (input.TryGetProperty(EmailMessageSchema.ConversationTopic, out var conversationTopic))
                data.Properties[vocabulary.ConversationTopic] = input.ConversationTopic.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.IsAssociated, out var isAssociated))
                data.Properties[vocabulary.IsAssociated] = input.IsAssociated.PrintIfAvailable();
            if (input.TryGetProperty(EmailMessageSchema.IsResponseRequested, out var isResponseRequested))
                data.Properties[vocabulary.IsResponseRequested] = input.IsResponseRequested.PrintIfAvailable();
            if (input.TryGetProperty(EmailMessageSchema.References, out var references))
                data.Properties[vocabulary.References] = input.References.PrintIfAvailable();
            if (input.TryGetProperty(EmailMessageSchema.IsDeliveryReceiptRequested, out var isDeliveryReceiptRequested))
                data.Properties[vocabulary.IsDeliveryReceiptRequested] = isDeliveryReceiptRequested.PrintIfAvailable();
                
            if (input.ReceivedBy != null)
                WriteAddressProperty(data, vocabulary.ReceivedBy, input.ReceivedBy);
            if (input.ReceivedRepresenting != null)
                WriteAddressProperty(data, vocabulary.ReceivedRepresenting, input.ReceivedRepresenting);

            if (input.ReplyTo != null)
            {
                if (input.ReplyTo.Count == 1)
                    WriteAddressProperty(data, vocabulary.ReplyTo, input.ReplyTo.First());
                else
                    data.Properties[vocabulary.ReplyTo] = string.Join(";", input.ReplyTo.Where(a => a.Address != null && MailAddressUtility.IsValid(a.Address)).Select(a => a.Address));
            }

            // From
            if (input.From != null)
            {
                var fromReference = AddCreatedBy(clue, input.From, vocabulary.From);
                data.LastChangedBy = fromReference ?? data.LastChangedBy;
                WriteAddressProperty(clue.Data.EntityData, vocabulary.From, input.From);
            }

            // Sender
            if (input.Sender != null)
            {
                AddCreatedBy(clue, input.Sender, vocabulary.Sender);
                WriteAddressProperty(clue.Data.EntityData, vocabulary.Sender, input.Sender);
            }

            // Recipients
            AddEmailAddressEdges(clue, input.BccRecipients, EntityEdgeType.Recipient);
            AddEmailAddressEdges(clue, input.CcRecipients, EntityEdgeType.Recipient);
            AddEmailAddressEdges(clue, input.ToRecipients, EntityEdgeType.Recipient);

            // Item vocabulary
            if (input.TryGetProperty(ItemSchema.Sensitivity, out var sensitivity))
                data.Properties[vocabulary.Sensitivity] = input.Sensitivity.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.Size, out var size))
                data.Properties[vocabulary.Size] = input.Size.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.Culture, out var culture))
                data.Properties[vocabulary.Culture] = input.Culture.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.Importance, out var importance))
                data.Properties[vocabulary.Importance] = input.Importance.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.InReplyTo, out var inReplyTo))
                data.Properties[vocabulary.InReplyTo] = input.InReplyTo.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.IsSubmitted, out var isSubmitted))
                data.Properties[vocabulary.IsSubmitted] = input.IsSubmitted.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.IsDraft, out var isDraft))
                data.Properties[vocabulary.IsDraft] = input.IsDraft.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.IsResend, out var isResend))
                data.Properties[vocabulary.IsResend] = input.IsResend.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.IsUnmodified, out var isUnmodified))
                data.Properties[vocabulary.IsUnmodified] = input.IsUnmodified.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.DateTimeCreated, out var dateTimeCreated))
                data.Properties[vocabulary.DateTimeCreated] = input.DateTimeCreated.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.ReminderDueBy, out var reminderDueBy))
                data.Properties[vocabulary.ReminderDueBy] = input.ReminderDueBy.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.IsReminderSet, out var isReminderSet))
                data.Properties[vocabulary.IsReminderSet] = input.IsReminderSet.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.ReminderMinutesBeforeStart, out var reminderMinutesBeforeStart))
                data.Properties[vocabulary.ReminderMinutesBeforeStart] = input.ReminderMinutesBeforeStart.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.DisplayCc, out var displayCc))
                data.Properties[vocabulary.DisplayCc] = input.DisplayCc.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.DisplayTo, out var displayTo))
                data.Properties[vocabulary.DisplayTo] = input.DisplayTo.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.HasAttachments, out var hasAttachments))
                data.Properties[vocabulary.HasAttachments] = input.HasAttachments.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.ItemClass, out var itemClass))
                data.Properties[vocabulary.ItemClass] = input.ItemClass.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.Subject, out var subject))
                data.Properties[vocabulary.Subject] = input.Subject.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.WebClientReadFormQueryString, out var webClientReadFormQueryString))
                data.Properties[vocabulary.WebClientReadFormQueryString] = input.WebClientReadFormQueryString.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.WebClientEditFormQueryString, out var webClientEditFormQueryString))
                data.Properties[vocabulary.WebClientEditFormQueryString] = input.WebClientEditFormQueryString.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.LastModifiedName, out var lastModifiedName))
                data.Properties[vocabulary.LastModifiedName] = input.LastModifiedName.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.LastModifiedTime, out var lastModifiedTime))
                data.Properties[vocabulary.LastModifiedTime] = input.LastModifiedTime.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.ConversationId, out var conversationId))
                data.Properties[vocabulary.ConversationId] = input.ConversationId?.UniqueId.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.RetentionDate, out var retentionDate))
                data.Properties[vocabulary.RetentionDate] = input.RetentionDate.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.Preview, out var preview))
                data.Properties[vocabulary.Preview] = input.Preview.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.IconIndex, out var iconIndex))
                data.Properties[vocabulary.IconIndex] = input.IconIndex.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.AllowedResponseActions, out var allowedResponseActions))
                data.Properties[vocabulary.AllowedResponseActions] = input.AllowedResponseActions.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.IsFromMe, out var isFromMe))
                data.Properties[vocabulary.IsFromMe] = input.IsFromMe.PrintIfAvailable();
            if (input.TryGetProperty(ItemSchema.EffectiveRights, out var effectiveRights))
                data.Properties[vocabulary.EffectiveRights] = input.EffectiveRights.PrintIfAvailable();

            data.Properties[vocabulary.IsAttachment] = input.IsAttachment.PrintIfAvailable();
            data.Properties[vocabulary.IsNew] = input.IsNew.PrintIfAvailable();

            if (input.Flag != null && input.Flag.FlagStatus != ItemFlagStatus.NotFlagged)
            {
                data.Properties[vocabulary.Flag.CompleteDate] = input.Flag?.CompleteDate.PrintIfAvailable();
                data.Properties[vocabulary.Flag.DueDate] = input.Flag?.DueDate.PrintIfAvailable();
                data.Properties[vocabulary.Flag.FlagStatus] = input.Flag?.FlagStatus.PrintIfAvailable();
                data.Properties[vocabulary.Flag.StartDate] = input.Flag?.StartDate.PrintIfAvailable();
            }

            // Categories
            {
                foreach (var category in ((IEnumerable<string>)input.Categories ?? new string[0]).Where(c => c != null))
                {
                    var parsedCategory = this.ParseCategory(category);
                    data.Tags.Add(new Tag(parsedCategory));
                }
            }

            // InternetMessageHeaders
            {
                if (input.Id != null)
                {
                    try
                    {
                        var message = BindItem(input, input.Service, new PropertySet(BasePropertySet.IdOnly, ItemSchema.InternetMessageHeaders));

                        if (message.InternetMessageHeaders != null)
                        {
                            var mailHeadersVocab = new MailHeadersVocabulary();
                            foreach (var header in message.InternetMessageHeaders)
                                data.Properties[mailHeadersVocab.KeyPrefix + mailHeadersVocab.KeySeparator + header.Name] = header.Value.PrintIfAvailable();
                        }
                    }
                    catch (Exception exc)
                    {
                        _state.Log.LogError("Failed to load item internet headers. ", exc);
                    }
                }
            }

            // ExtendedProperties
            {
                var extendedPropertiesVocabulary = new ExtendedPropertiesVocabulary();
                foreach (var extendedProperty in (IEnumerable<ExtendedProperty>)input.ExtendedProperties ?? new ExtendedProperty[0])
                    data.Properties[extendedPropertiesVocabulary.KeyPrefix + extendedPropertiesVocabulary.KeySeparator + extendedProperty.PropertyDefinition.Name] = extendedProperty.Value.PrintIfAvailable();
            }

            // Body
            {
                if (input.Id != null)
                {
                    try
                    {
                        var version = (int)input.Service.RequestedServerVersion;

                        PropertySet itempropertyset = version > (int)ExchangeVersion.Exchange2010
                                                        ? new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.Body, ItemSchema.UniqueBody, ItemSchema.Attachments)
                                                        : new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.Body, ItemSchema.Attachments);

                        itempropertyset.RequestedBodyType = BodyType.Text;

                        if (version > (int)ExchangeVersion.Exchange2010)
                            itempropertyset.RequestedUniqueBodyType = BodyType.Text;

                        input.Load(itempropertyset);

                        // TODO: Save this as HTML
                        if (input.Body != null)
                        {
                            if (input.Body.Text != null)
                                data.Properties[vocabulary.Body] = input.Body.Text;

                            data.Encoding = input.Body?.BodyType.ToString() ?? data.Encoding;

                            try
                            {
                                IndexContent(input.Body.Text, clue);
                            }
                            catch (Exception exc)
                            {
                                _state.Log.LogError("Failed to index body", exc);
                            }
                        }

                        data.Properties[vocabulary.UniqueBody] = input.UniqueBody?.Text.PrintIfAvailable();

                        // Ignored
                        // data.Properties[vocabulary.NormalizedBody] = value.NormalizedBody?.Text;
                    }
                    catch (Exception exc)
                    {
                        _state.Log.LogError("Failed to index body", exc);
                    }
                }
            }

            // Attachments
            {
                try
                {
                    if (input.Attachments != null)
                    {
                        var attachments = input.Attachments.Select(attachment => new AttachmentModel(attachment, input, input.Service));
                        foreach (var attachment in AttachmentModel.FilterAttachments(attachments, _state))
                        {
                            var from = new EntityReference(new EntityCode(EntityType.Files.File, "ExchangeSharedMailbox", attachment.Attachment.Id), attachment.Attachment.Name);
                            var to = new EntityReference(clue.OriginEntityCode);
                            var edge = new EntityEdge(@from, to, EntityEdgeType.PartOf);

                            clue.Data.EntityData.IncomingEdges.Add(edge);
                        }
                    }
                }
                catch (Exception exception)
                {
                    _state.Log.LogWarning("Could not index Attachment", exception);
                }
            }

            // Edges
            if (input.ParentFolderId != null)
                _factory.CreateOutgoingEntityReference(clue, EntityType.Infrastructure.Folder, EntityEdgeType.PartOf, input, input.ParentFolderId.UniqueId);

            //{
            //    if (input.ParentFolderId != null && !this.IsFilteredFolder(item.Folder))
            //    {
            //        var parentCode = new EntityCode(EntityType.Infrastructure.Folder, SkypeConstants.CodeOrigin, input.ParentFolderId.UniqueId);

            //        var edge = new EntityEdge(
            //            EntityReference.CreateByKnownCode(clue.OriginEntityCode),
            //            EntityReference.CreateByKnownCode(parentCode, input.Folder?.DisplayName),
            //            EntityEdgeType.Parent);

            //        data.OutgoingEdges.Add(edge);
            //    }
            //}

            return clue;
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

        private void IndexContent(string content, Clue clue)
        {
            if (string.IsNullOrEmpty(content))
                return;

            try
            {
                using (var tempFile = new TemporaryFile("item.html"))
                {
                    byte[] bytes = new byte[content.Length * sizeof(char)];
                    Buffer.BlockCopy(content.ToCharArray(), 0, bytes, 0, bytes.Length);

                    using (var stream = new MemoryStream(bytes))
                    using (var fileStream = File.Create(tempFile.FilePath))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(fileStream);
                    }

                    FileCrawlingUtility.ExtractContents(tempFile, clue.Data, clue, _state, _appContext);
                }
            }
            catch (Exception exception)
            {
                _state.Log.LogError("Error Indexing Content", exception);
            }
        }

        protected Item BindItem(Item item, ExchangeService service, PropertySet propertySet)
        {
            if (item.GetType() == typeof(Appointment))
                return Appointment.Bind(service, item.Id, propertySet).Result;
            else if (item.GetType() == typeof(Task))
                return Task.Bind(service, item.Id, propertySet).Result;
            else if (item.GetType() == typeof(Contact))
                return Contact.Bind(service, item.Id, propertySet).Result;
            else if (item.GetType() == typeof(ContactGroup))
                return ContactGroup.Bind(service, item.Id, propertySet).Result;
            else if (item.GetType() == typeof(PostItem))
                return PostItem.Bind(service, item.Id, propertySet).Result;
            else if (item.GetType() == typeof(MeetingCancellation))
                return MeetingCancellation.Bind(service, item.Id, propertySet).Result;
            else if (item.GetType() == typeof(MeetingRequest))
                return MeetingRequest.Bind(service, item.Id, propertySet).Result;
            else if (item.GetType() == typeof(MeetingResponse))
                return MeetingResponse.Bind(service, item.Id, propertySet).Result;
            else if (item.GetType() == typeof(MeetingMessage))
                return MeetingMessage.Bind(service, item.Id, propertySet).Result;
            else if (item.GetType() == typeof(EmailMessage))
                return EmailMessage.Bind(service, item.Id, propertySet).Result;
            else
                throw new Exception("Unknown Exchange Item type: " + item.GetType().FullName);
        }

        protected PersonReference AddCreatedBy(
            Clue clue,
            EmailAddress address,
            VocabularyKey vocabularyKey)
        {
            if (address == null)
                return null;

            PersonReference personReference;

            if (!string.IsNullOrEmpty(address.Address))
                personReference = new PersonReference(address.Name, new EntityCode(EntityType.Infrastructure.User, SkypeConstants.CodeOrigin, address.Address));
            else
                personReference = new PersonReference(address.Name);

            clue.Data.EntityData.Authors.Add(personReference);
            clue.Data.EntityData.LastChangedBy = personReference;

            if (address.Address != null && !clue.Data.EntityData.OutgoingEdges.Any(e => e.EdgeType.Is(EntityEdgeType.CreatedBy)))
            {
                var fromCode = new EntityCode(EntityType.Infrastructure.User, SkypeConstants.CodeOrigin, address.Address);

                var fromEdge = new EntityEdge(
                    EntityReference.CreateByKnownCode(clue.OriginEntityCode),
                    EntityReference.CreateByKnownCode(fromCode, address.Name),
                    EntityEdgeType.CreatedBy);

                clue.Data.EntityData.OutgoingEdges.Add(fromEdge);
            }

            return personReference;
        }

        private string ParseCategory(string category)
        {
            var colourRegex = new Regex("^#(?:[0-9a-fA-F]{3}){1,2}$");

            if (colourRegex.IsMatch(category))
            {
                var col = System.Drawing.ColorTranslator.FromHtml(category);

                return $"{col.Name} Category";
            }

            return category;
        }

        protected void WriteAddressProperty(
            IEntityMetadataPart data,
            VocabularyKey vocabularyKey,
            EmailAddress address)
        {
            if (address == null)
                return;

            if (address.Address != null)
                data.Properties[vocabularyKey] = $"{address.Name} {address.Address}";
            else
                data.Properties[vocabularyKey] = address.Name;
        }

        protected void AddEmailAddressEdge(
           Clue clue,
           EmailAddress address,
           EntityEdgeType edgeType)
        {
            if (address?.Id != null)
            {
                // TODO
                var entityCode = new EntityCode(
                    EntityType.Infrastructure.User,
                    SkypeConstants.CodeOrigin,
                    address.Id.UniqueId);

                var entityEdge = new EntityEdge(
                    EntityReference.CreateByKnownCode(clue.OriginEntityCode),
                    EntityReference.CreateByKnownCode(entityCode, address.Name),
                    edgeType);

                clue.Data.EntityData.OutgoingEdges.Add(entityEdge);
            }
            else if (address?.Address != null)
            {
                var entityCode = new EntityCode(
                    EntityType.Infrastructure.User,
                    SkypeConstants.CodeOrigin,
                    address.Address);

                var entityEdge = new EntityEdge(
                    EntityReference.CreateByKnownCode(clue.OriginEntityCode),
                    EntityReference.CreateByKnownCode(entityCode, address.Name),
                    edgeType);

                clue.Data.EntityData.OutgoingEdges.Add(entityEdge);
            }

        }

        protected void AddEmailAddressEdges(Clue clue, IEnumerable<EmailAddress> addresses, EntityEdgeType edgeType)
        {
            if (addresses == null)
                return;

            foreach (var recipient in addresses)
                AddEmailAddressEdge(clue, recipient, edgeType);
        }

    }
}
