using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using CluedIn.Core;
using CluedIn.Core.Agent.Jobs;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Data.Vocabularies;
using CluedIn.Core.IO;
using CluedIn.Crawling.ExchangeSharedMailbox.Constants;
using CluedIn.Crawling.ExchangeSharedMailbox.Factories;
using CluedIn.Crawling.ExchangeSharedMailbox.Helpers;
using CluedIn.Crawling.ExchangeSharedMailbox.Models;
using CluedIn.Crawling.ExchangeSharedMailbox.Vocabularies;

using Microsoft.Exchange.WebServices.Data;

namespace CluedIn.Crawling.ExchangeSharedMailbox.Subjects
{
    public class ItemObserverBase
    {
        private readonly ApplicationContext appContext;

        private readonly AgentJobProcessorState<ExchangeSharedMailboxCrawlJobData> state;

        private readonly IClueFactory factory;

        protected ItemObserverBase([NotNull] ApplicationContext appContext, 
                                   [NotNull] AgentJobProcessorState<ExchangeSharedMailboxCrawlJobData> state,
                                   [NotNull] IClueFactory factory)
        {
            this.appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            this.state      = state ?? throw new ArgumentNullException(nameof(state));
            this.factory    = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        protected void PopulateItem(
            Clue clue, 
            IItemObjectModel<Item> item, 
            ExchangeSharedMailboxItemVocabulary vocabulary, 
            ExchangeService service)
        {
            var value = item.Object;
            var data = clue.Data.EntityData;

            data.Name           = value.ExPrintIfAvailable(v => v.Subject) ?? data.Name;
            data.CreatedDate    = value.ExGetIfAvailable(v => (DateTime?)v.DateTimeCreated, null)
                               ?? value.ExGetIfAvailable(v => (DateTime?)v.DateTimeReceived, null);
            data.ModifiedDate   = value.ExGetIfAvailable(v => (DateTime?)v.LastModifiedTime, null)
                               ?? value.ExGetIfAvailable(v => (DateTime?)v.DateTimeSent, null);

            data.Culture        = value.ExGetIfAvailable(v => !string.IsNullOrEmpty(v.Culture) ? new CultureInfo(v.Culture) : data.Culture, data.Culture);

            // LastChangedBy
            if ((item.Folder != null && item.Folder.IsFilteredFolder() && item.Object is Contact) == false)
            {
                var lastModifiedName = value.ExGetIfAvailable(v => v.LastModifiedName, null);
                if (!string.IsNullOrEmpty(lastModifiedName))
                    data.LastChangedBy = new PersonReference(new EntityReference(EntityType.Infrastructure.User, lastModifiedName));
            }

            // Properties
            data.Properties[vocabulary.IsAttachment]                    = value.ExPrintIfTrue(v => v.IsAttachment);
            //data.Properties[vocabulary.IsNew]                           = value.ExPrintIfTrue(v => v.IsNew);
            data.Properties[vocabulary.Sensitivity]                     = value.ExPrintIfAvailable(v => v.Sensitivity);
            data.Properties[vocabulary.DateTimeReceived]                = value.ExPrintIfAvailable(v => v.DateTimeReceived);
            data.Properties[vocabulary.Size]                            = value.ExPrintIfAvailable(v => v.Size);
            data.Properties[vocabulary.Culture]                         = value.ExPrintIfAvailable(v => v.Culture);
            data.Properties[vocabulary.Importance]                      = value.ExPrintIfAvailable(v => v.Importance);
            data.Properties[vocabulary.InReplyTo]                       = value.ExPrintIfAvailable(v => v.InReplyTo);
            data.Properties[vocabulary.IsSubmitted]                     = value.ExPrintIfTrue(v => v.IsSubmitted);
            data.Properties[vocabulary.IsAssociated]                    = value.ExPrintIfTrue(v => v.IsAssociated);
            data.Properties[vocabulary.IsDraft]                         = value.ExPrintIfTrue(v => v.IsDraft);
            //data.Properties[vocabulary.IsFromMe]                        = value.ExPrintIfTrue(v => v.IsFromMe);
            data.Properties[vocabulary.IsResend]                        = value.ExPrintIfTrue(v => v.IsResend);
            data.Properties[vocabulary.IsUnmodified]                    = value.ExPrintIfTrue(v => v.IsUnmodified);
            data.Properties[vocabulary.DateTimeSent]                    = value.ExPrintIfAvailable(v => v.DateTimeSent);
            data.Properties[vocabulary.DateTimeCreated]                 = value.ExPrintIfAvailable(v => v.DateTimeCreated);
            data.Properties[vocabulary.ReminderDueBy]                   = value.ExPrintIfAvailable(v => v.ReminderDueBy);
            data.Properties[vocabulary.IsReminderSet]                   = value.ExPrintIfTrue(v => v.IsReminderSet);
            data.Properties[vocabulary.ReminderMinutesBeforeStart]      = value.ExPrintIfAvailable(v => v.ReminderMinutesBeforeStart);
            data.Properties[vocabulary.DisplayCc]                       = value.ExPrintIfAvailable(v => v.DisplayCc);
            data.Properties[vocabulary.DisplayTo]                       = value.ExPrintIfAvailable(v => v.DisplayTo);
            data.Properties[vocabulary.HasAttachments]                  = value.ExPrintIfTrue(v => v.HasAttachments);
            data.Properties[vocabulary.ItemClass]                       = value.ExPrintIfAvailable(v => v.ItemClass);
            data.Properties[vocabulary.Subject]                         = value.ExPrintIfAvailable(v => v.Subject);
            data.Properties[vocabulary.WebClientReadFormQueryString]    = value.ExPrintIfAvailable(v => v.WebClientReadFormQueryString);
            data.Properties[vocabulary.WebClientEditFormQueryString]    = value.ExPrintIfAvailable(v => v.WebClientEditFormQueryString);
            //data.Properties[vocabulary.EffectiveRights]                 = value.ExPrintIfAvailable(v => v.EffectiveRights);
            data.Properties[vocabulary.LastModifiedName]                = value.ExPrintIfAvailable(v => v.LastModifiedName);
            data.Properties[vocabulary.LastModifiedTime]                = value.ExPrintIfAvailable(v => v.LastModifiedTime);
            data.Properties[vocabulary.ConversationId]                  = value.ExPrintIfAvailable(v => v.ConversationId?.UniqueId);
            data.Properties[vocabulary.RetentionDate]                   = value.ExPrintIfAvailable(v => v.RetentionDate);
            data.Properties[vocabulary.Preview]                         = value.ExPrintIfAvailable(v => v.Preview);
            data.Properties[vocabulary.IconIndex]                       = value.ExPrintIfAvailable(v => v.IconIndex);
            data.Properties[vocabulary.AllowedResponseActions]          = value.ExPrintIfAvailable(v => v.AllowedResponseActions);

            if (value.ExGetIfAvailable(v => v.Flag?.FlagStatus, ItemFlagStatus.NotFlagged) != ItemFlagStatus.NotFlagged)
            {
                data.Properties[vocabulary.Flag.CompleteDate]               = value.ExPrintIfAvailable(v => v.Flag?.CompleteDate);
                data.Properties[vocabulary.Flag.DueDate]                    = value.ExPrintIfAvailable(v => v.Flag?.DueDate);
                data.Properties[vocabulary.Flag.FlagStatus]                 = value.ExPrintIfAvailable(v => v.Flag?.FlagStatus);
                data.Properties[vocabulary.Flag.StartDate]                  = value.ExPrintIfAvailable(v => v.Flag?.StartDate);
            }

            if (value.ParentFolderId != null && !this.IsFilteredFolder(item.Folder))
            {
                data.Properties[vocabulary.UniqueId]                    = value.ExPrintIfAvailable(v => v.Id?.UniqueId);
                data.Properties[vocabulary.ParentFolderId]              = value.ExPrintIfAvailable(v => v.ParentFolderId);
            }

            // Ignored:
            // StoreEntryId
            // InstanceKey
            // PolicyTag
            // ArchiveTag
            // MimeContent

            // Categories
            {
                foreach (var category in ((IEnumerable<string>)value.Categories ?? new string[0]).Where(c => c != null))
                {
                    var parsedCategory = this.ParseCategory(category);
                    data.Tags.Add(new Tag(parsedCategory));
                }
            }

            // InternetMessageHeaders
            {
                if (value.Id != null)
                {
                    try
                    {
                        var message = this.BindItem(value, service, new PropertySet(BasePropertySet.IdOnly, ItemSchema.InternetMessageHeaders));

                        if (message.InternetMessageHeaders != null)
                        {
                            foreach (var header in message.InternetMessageHeaders)
                                data.Properties[ExchangeSharedMailboxVocabulary.MailHeaders.KeyPrefix + ExchangeSharedMailboxVocabulary.MailHeaders.KeySeparator + header.Name] = header.ExPrintIfAvailable(v => v.Value);
                        }
                    }
                    catch (Exception exc)
                    {
                        this.state.Log.Error(() => "Failed to load item internet headers.", exc);
                    }
                }
            }

            // ExtendedProperties
            {
                foreach (var extendedProperty in (IEnumerable<ExtendedProperty>)value.ExtendedProperties ?? new ExtendedProperty[0])
                    data.Properties[ExchangeSharedMailboxVocabulary.ExtendedProperties.KeyPrefix + ExchangeSharedMailboxVocabulary.ExtendedProperties.KeySeparator + extendedProperty.PropertyDefinition.Name] = extendedProperty.ExPrintIfAvailable(v => v.Value);
            }

            // Body
            {
                if (value.Id != null)
                {
                    try
                    {
                        if (value.ExGetIfAvailable(v => v.Body, null) != null)
                        {

                        }

                        var version = (int)value.Service.RequestedServerVersion;

                        PropertySet itempropertyset = version > (int)ExchangeVersion.Exchange2010 
                                                        ? new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.Body, ItemSchema.UniqueBody, ItemSchema.Attachments)
                                                        : new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.Body, ItemSchema.Attachments);

                        itempropertyset.RequestedBodyType       = BodyType.Text;

                        if (version > (int)ExchangeVersion.Exchange2010)
                            itempropertyset.RequestedUniqueBodyType = BodyType.Text;

                        value.ExLoad(this.state, itempropertyset);

                        // TODO: Save this as HTML
                        if (value.Body != null)
                        {
                            if (value.Body.Text != null)
                                data.Properties[vocabulary.Body] = value.Body.Text;

                            data.Encoding = value.Body?.BodyType.ToString() ?? data.Encoding;

                            try
                            {
                                this.IndexContent(value.Body.Text, clue);
                            }
                            catch (Exception exc)
                            {
                                this.state.Log.Error(() => "Failed to index body", exc);
                            }
                        }

                        data.Properties[vocabulary.UniqueBody] = value.ExPrintIfAvailable(v => v.UniqueBody?.Text);

                        // Ignored
                        // data.Properties[vocabulary.NormalizedBody] = value.NormalizedBody?.Text;
                    }
                    catch (Exception exc)
                    {
                        this.state.Log.Error(() => "Failed to index body", exc);
                    }
                }
            }

            // Attachments
            {
                try
                {
                    if (item.Attachments != null)
                    {
                        foreach (var attachment in AttachmentModel.FilterAttachments(item.Attachments, this.state))
                        {
                            var from = new EntityReference(new EntityCode(EntityType.Files.File, "ExchangeSharedMailbox", attachment.Attachment.Id), attachment.Attachment.Name);
                            var to   = new EntityReference(clue.OriginEntityCode);
                            var edge = new EntityEdge(@from, to, EntityEdgeType.PartOf);

                            clue.Data.EntityData.IncomingEdges.Add(edge);
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.state.Log.Warn(() => "Could not index Attachment", exception);
                }
            }

            // EntityExtractionResult
            // TODO
            
            // Edges
            {
                if (value.ParentFolderId != null && !this.IsFilteredFolder(item.Folder))
                {
                    var parentCode = new EntityCode(EntityType.Infrastructure.Folder, ExchangeSharedMailboxNameConstants.CodeOrigin, value.ParentFolderId.UniqueId);

                    var edge = new EntityEdge(
                        EntityReference.CreateByKnownCode(clue.OriginEntityCode),
                        EntityReference.CreateByKnownCode(parentCode, item.Folder?.DisplayName),
                        EntityEdgeType.Parent);

                    data.OutgoingEdges.Add(edge);
                }
            }
        }

        private bool IsFilteredFolder(FolderModel itemFolder)
        {
            if (itemFolder == null)
                return true;

            return itemFolder.IsFilteredFolder();
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

                    FileCrawlingUtility.ExtractContents(tempFile, clue.Data, clue, this.state, this.appContext);
                }
            }
            catch (Exception exception)
            {
                this.state.Log.Error(() => "Error Indexing Content", exception);
            }
        }

        protected Item BindItem(Item item, ExchangeService service, PropertySet propertySet)
        {
            if (item.GetType() == typeof(Microsoft.Exchange.WebServices.Data.Appointment))
                return Appointment.Bind(service, item.Id, propertySet);
            else if (item.GetType() == typeof(Microsoft.Exchange.WebServices.Data.Task))
                return Task.Bind(service, item.Id, propertySet);
            else if (item.GetType() == typeof(Microsoft.Exchange.WebServices.Data.Contact))
                return Contact.Bind(service, item.Id, propertySet);
            else if (item.GetType() == typeof(Microsoft.Exchange.WebServices.Data.ContactGroup))
                return ContactGroup.Bind(service, item.Id, propertySet);
            else if (item.GetType() == typeof(Microsoft.Exchange.WebServices.Data.PostItem))
                return PostItem.Bind(service, item.Id, propertySet);
            else if (item.GetType() == typeof(Microsoft.Exchange.WebServices.Data.MeetingCancellation))
                return MeetingCancellation.Bind(service, item.Id, propertySet);
            else if (item.GetType() == typeof(Microsoft.Exchange.WebServices.Data.MeetingRequest))
                return MeetingRequest.Bind(service, item.Id, propertySet);
            else if (item.GetType() == typeof(Microsoft.Exchange.WebServices.Data.MeetingResponse))
                return MeetingResponse.Bind(service, item.Id, propertySet);
            else if (item.GetType() == typeof(Microsoft.Exchange.WebServices.Data.MeetingMessage))
                return MeetingMessage.Bind(service, item.Id, propertySet);
            else if (item.GetType() == typeof(Microsoft.Exchange.WebServices.Data.EmailMessage))
                return EmailMessage.Bind(service, item.Id, propertySet);
            else
                throw new Exception("Unknown Exchange Item type: " + item.GetType().FullName);
        }

        protected void AddEmailAddressEdges(Clue clue, IEnumerable<EmailAddress> addresses, EntityEdgeType edgeType)
        {
            if (addresses == null)
                return;

            foreach (var recipient in addresses)
                AddEmailAddressEdge(clue, recipient, edgeType);
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
                    ExchangeSharedMailboxNameConstants.CodeOrigin,
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
                    ExchangeSharedMailboxNameConstants.CodeOrigin,
                    address.Address);

                var entityEdge = new EntityEdge(
                    EntityReference.CreateByKnownCode(clue.OriginEntityCode),
                    EntityReference.CreateByKnownCode(entityCode, address.Name),
                    edgeType);

                clue.Data.EntityData.OutgoingEdges.Add(entityEdge);
            }
            
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
                personReference = new PersonReference(address.Name, new EntityCode(EntityType.Infrastructure.User, ExchangeSharedMailboxNameConstants.CodeOrigin, address.Address));
            else
                personReference = new PersonReference(address.Name);

            clue.Data.EntityData.Authors.Add(personReference);
            clue.Data.EntityData.LastChangedBy = personReference;

            this.WriteAddressProperty(clue.Data.EntityData, vocabularyKey, address);

            if (address.Address != null && !clue.Data.EntityData.OutgoingEdges.Any(e => e.EdgeType.Is(EntityEdgeType.CreatedBy)))
            {
                var fromCode = new EntityCode(EntityType.Infrastructure.User, ExchangeSharedMailboxNameConstants.CodeOrigin, address.Address);

                var fromEdge = new EntityEdge(
                    EntityReference.CreateByKnownCode(clue.OriginEntityCode),
                    EntityReference.CreateByKnownCode(fromCode, address.Name),
                    EntityEdgeType.CreatedBy);

                clue.Data.EntityData.OutgoingEdges.Add(fromEdge);
            }

            return personReference;
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
    }
}
