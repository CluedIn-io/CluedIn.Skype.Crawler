using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.Crawling.Skype.Vocabularies
{
    public class ExchangeSharedMailboxItemVocabulary : SimpleVocabulary
    {
        public ExchangeSharedMailboxItemVocabulary()
        {
            VocabularyName = "Exchange Item";
            KeyPrefix = "exchange.item";
            KeySeparator = ".";
            Grouping = EntityType.List.Item;

            AddGroup("Exchange Item Details", group =>
            {
                Importance = group.Add(new VocabularyKey("importance"));
                IsDraft = group.Add(new VocabularyKey("isDraft", VocabularyKeyDataType.Boolean, VocabularyKeyVisibility.Hidden));
                HasAttachments = group.Add(new VocabularyKey("hasAttachments", VocabularyKeyDataType.Boolean));
                Body = group.Add(new VocabularyKey("body", VocabularyKeyDataType.Html));
                UniqueBody = group.Add(new VocabularyKey("uniqueBody", VocabularyKeyDataType.Html));
                NormalizedBody = group.Add(new VocabularyKey("normalizedBody", VocabularyKeyDataType.Html));
                TextBody = group.Add(new VocabularyKey("textBody", VocabularyKeyDataType.Html));
                ItemClass = group.Add(new VocabularyKey("itemClass"));
                IsUnmodified = group.Add(new VocabularyKey("isUnmodified", VocabularyKeyDataType.Boolean));
                IsSubmitted = group.Add(new VocabularyKey("isSubmitted", VocabularyKeyDataType.Boolean));
                IsResend = group.Add(new VocabularyKey("isResend", VocabularyKeyDataType.Boolean));
                IsReminderSet = group.Add(new VocabularyKey("isReminderSet", VocabularyKeyDataType.Boolean));
                IsNew = group.Add(new VocabularyKey("isNew", VocabularyKeyDataType.Boolean));
                IsFromMe = group.Add(new VocabularyKey("isFromMe", VocabularyKeyDataType.Boolean));
                IsAttachment = group.Add(new VocabularyKey("isAttachment", VocabularyKeyDataType.Boolean));
                IsAssociated = group.Add(new VocabularyKey("isAssociated", VocabularyKeyDataType.Boolean));
                InReplyTo = group.Add(new VocabularyKey("inReplyTo"));
                IconIndex = group.Add(new VocabularyKey("iconIndex"));
                EffectiveRights = group.Add(new VocabularyKey("effectiveRights"));
                DisplayTo = group.Add(new VocabularyKey("displayTo"));
                DisplayCc = group.Add(new VocabularyKey("displayCc"));
                Culture = group.Add(new VocabularyKey("culture"));
                AllowedResponseActions = group.Add(new VocabularyKey("allowedResponseActions"));
                LastModifiedName = group.Add(new VocabularyKey("lastModifiedName"));
                LastModifiedTime = group.Add(new VocabularyKey("lastModifiedTime", VocabularyKeyDataType.DateTime));
                Preview = group.Add(new VocabularyKey("preview"));
                ReminderDueBy = group.Add(new VocabularyKey("reminderDueBy", VocabularyKeyDataType.DateTime));
                ReminderMinutesBeforeStart = group.Add(new VocabularyKey("reminderMinutesBeforeStart"));
                RetentionDate = group.Add(new VocabularyKey("retentionDate"));
                Sensitivity = group.Add(new VocabularyKey("sensitivity"));
                Size = group.Add(new VocabularyKey("size"));
                WebClientEditFormQueryString = group.Add(new VocabularyKey("webClientEditFormQueryString"));
                WebClientReadFormQueryString = group.Add(new VocabularyKey("webClientReadFormQueryString"));
                Subject = group.Add(new VocabularyKey("subject"));
                DateTimeSent = group.Add(new VocabularyKey("dateTimeSent", VocabularyKeyDataType.DateTime));
                DateTimeReceived = group.Add(new VocabularyKey("dateTimeReceived", VocabularyKeyDataType.DateTime));
                DateTimeCreated = group.Add(new VocabularyKey("dateTimeCreated", VocabularyKeyDataType.DateTime));
                ConversationId = group.Add(new VocabularyKey("conversationId", VocabularyKeyVisibility.ExcludeFromHashing));
                UniqueId = group.Add(new VocabularyKey("uniqueId", VocabularyKeyVisibility.Hidden));
                ParentFolderId = group.Add(new VocabularyKey("parentFolderId", VocabularyKeyVisibility.Hidden));
                Flag = group.Add(new ExchangeSharedMailboxFlagVocabulary().AsCompositeKey("flag"));
            });

            AddMapping(DateTimeCreated, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInDates.CreatedDate);
            AddMapping(LastModifiedTime, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInDates.ModifiedDate);
        }

        public VocabularyKey DateTimeSent { get; internal set; }
        public VocabularyKey DateTimeReceived { get; internal set; }
        public VocabularyKey DateTimeCreated { get; internal set; }
        public VocabularyKey ConversationId { get; internal set; }
        public VocabularyKey Subject { get; internal set; }
        public VocabularyKey Size { get; internal set; }
        public VocabularyKey WebClientEditFormQueryString { get; internal set; }
        public VocabularyKey WebClientReadFormQueryString { get; internal set; }
        public VocabularyKey LastModifiedName { get; internal set; }
        public VocabularyKey LastModifiedTime { get; internal set; }
        public VocabularyKey Preview { get; internal set; }
        public VocabularyKey ReminderDueBy { get; internal set; }
        public VocabularyKey ReminderMinutesBeforeStart { get; internal set; }
        public VocabularyKey RetentionDate { get; internal set; }
        public VocabularyKey Sensitivity { get; internal set; }
        public VocabularyKey IsFromMe { get; internal set; }
        public VocabularyKey IsNew { get; internal set; }
        public VocabularyKey IsReminderSet { get; internal set; }
        public VocabularyKey IsResend { get; internal set; }
        public VocabularyKey IsSubmitted { get; internal set; }
        public VocabularyKey IsUnmodified { get; internal set; }
        public VocabularyKey ItemClass { get; internal set; }
        public VocabularyKey AllowedResponseActions { get; internal set; }
        public VocabularyKey Culture { get; internal set; }
        public VocabularyKey DisplayCc { get; internal set; }
        public VocabularyKey DisplayTo { get; internal set; }
        public VocabularyKey EffectiveRights { get; internal set; }
        public VocabularyKey IconIndex { get; internal set; }
        public VocabularyKey InReplyTo { get; internal set; }
        public VocabularyKey IsAssociated { get; internal set; }
        public VocabularyKey IsAttachment { get; internal set; }
        public VocabularyKey Importance { get; protected set; }
        public VocabularyKey IsDraft { get; protected set; }
        public VocabularyKey HasAttachments { get; protected set; }
        public VocabularyKey Body { get; protected set; }
        public VocabularyKey UniqueBody { get; protected set; }
        public VocabularyKey NormalizedBody { get; protected set; }
        public VocabularyKey TextBody { get; protected set; }
        public VocabularyKey UniqueId { get; internal set; }
        public VocabularyKey ParentFolderId { get; internal set; }
        public ExchangeSharedMailboxFlagVocabulary Flag { get; internal set; }
    }
}
