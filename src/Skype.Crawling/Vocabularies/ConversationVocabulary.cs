using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.Crawling.Skype.Vocabularies
{
    public class ConversationVocabulary : SimpleVocabulary
    {
        public ConversationVocabulary()
        {
            VocabularyName = "Skype Conversation";
            KeyPrefix = "skype.conversation";
            KeySeparator = ".";
            Grouping = EntityType.Mail;

            AddGroup("Skype Conversation Details", group => {

                Importance = group.Add(new VocabularyKey("Importance", VocabularyKeyDataType.Text));
                Categories = group.Add(new VocabularyKey("Categories", VocabularyKeyDataType.Text));
                FlagStatus = group.Add(new VocabularyKey("FlagStatus", VocabularyKeyDataType.Text));
                GlobalCategories = group.Add(new VocabularyKey("GlobalCategories", VocabularyKeyDataType.Text));
                GlobalFlagStatus = group.Add(new VocabularyKey("GlobalFlagStatus", VocabularyKeyDataType.Text));
                GlobalHasAttachments = group.Add(new VocabularyKey("GlobalHasAttachments", VocabularyKeyDataType.Boolean));
                GlobalHasIrm = group.Add(new VocabularyKey("GlobalHasIrm", VocabularyKeyDataType.Boolean));
                GlobalImportance = group.Add(new VocabularyKey("GlobalImportance", VocabularyKeyDataType.Text));
                GlobalItemClasses = group.Add(new VocabularyKey("GlobalItemClasses", VocabularyKeyDataType.Text));
                GlobalItemIds = group.Add(new VocabularyKey("GlobalItemIds", VocabularyKeyDataType.Json));
                GlobalLastDeliveryTime = group.Add(new VocabularyKey("GlobalLastDeliveryTime", VocabularyKeyDataType.DateTime));
                GlobalMessageCount = group.Add(new VocabularyKey("GlobalMessageCount", VocabularyKeyDataType.Text));
                GlobalSize = group.Add(new VocabularyKey("GlobalSize", VocabularyKeyDataType.Text));
                GlobalUniqueRecipients = group.Add(new VocabularyKey("GlobalUniqueRecipients", VocabularyKeyDataType.Text));
                GlobalUniqueSenders = group.Add(new VocabularyKey("GlobalUniqueSenders", VocabularyKeyDataType.Text));
                GlobalUniqueUnreadSenders = group.Add(new VocabularyKey("GlobalUniqueUnreadSenders", VocabularyKeyDataType.Text));
                GlobalUnreadCount = group.Add(new VocabularyKey("GlobalUnreadCount", VocabularyKeyDataType.Text));
                HasIrm = group.Add(new VocabularyKey("HasIrm", VocabularyKeyDataType.Text));
                HasAttachments = group.Add(new VocabularyKey("HasAttachments", VocabularyKeyDataType.Boolean));
                InstanceKey = group.Add(new VocabularyKey("InstanceKey", VocabularyKeyDataType.Text));
                IsDirty = group.Add(new VocabularyKey("IsDirty", VocabularyKeyDataType.Boolean));
                IsNew = group.Add(new VocabularyKey("IsNew", VocabularyKeyDataType.Boolean));
                ItemClasses = group.Add(new VocabularyKey("ItemClasses", VocabularyKeyDataType.Text));
                ItemIds = group.Add(new VocabularyKey("ItemIds", VocabularyKeyDataType.Json));
                LastDeliveryTime = group.Add(new VocabularyKey("LastDeliveryTime", VocabularyKeyDataType.DateTime));
                LastModifiedTime = group.Add(new VocabularyKey("LastModifiedTime", VocabularyKeyDataType.DateTime));
                MessageCount = group.Add(new VocabularyKey("MessageCount", VocabularyKeyDataType.Text));
                Size = group.Add(new VocabularyKey("Size", VocabularyKeyDataType.Text));
                Topic = group.Add(new VocabularyKey("Topic", VocabularyKeyDataType.Text));
                UniqueRecipients = group.Add(new VocabularyKey("UniqueRecipients", VocabularyKeyDataType.Text));
                UniqueSenders = group.Add(new VocabularyKey("UniqueSenders", VocabularyKeyDataType.Text));
                UniqueUnreadSenders = group.Add(new VocabularyKey("UniqueUnreadSenders", VocabularyKeyDataType.Text));
                UnreadCount = group.Add(new VocabularyKey("UnreadCount", VocabularyKeyDataType.Text));
            });
        }

        public VocabularyKey Importance { get; set; }
        public VocabularyKey Categories { get; set; }
        public VocabularyKey FlagStatus { get; set; }
        public VocabularyKey GlobalCategories { get; set; }
        public VocabularyKey GlobalFlagStatus { get; set; }
        public VocabularyKey GlobalHasAttachments { get; set; }
        public VocabularyKey GlobalHasIrm { get; set; }
        public VocabularyKey GlobalImportance { get; set; }
        public VocabularyKey GlobalItemClasses { get; set; }
        public VocabularyKey GlobalItemIds { get; set; }
        public VocabularyKey GlobalLastDeliveryTime { get; set; }
        public VocabularyKey GlobalMessageCount { get; set; }
        public VocabularyKey GlobalSize { get; set; }
        public VocabularyKey GlobalUniqueRecipients { get; set; }
        public VocabularyKey GlobalUniqueSenders { get; set; }
        public VocabularyKey GlobalUniqueUnreadSenders { get; set; }
        public VocabularyKey GlobalUnreadCount { get; set; }
        public VocabularyKey HasIrm { get; set; }
        public VocabularyKey HasAttachments { get; set; }
        public VocabularyKey InstanceKey { get; set; }
        public VocabularyKey IsDirty { get; set; }
        public VocabularyKey IsNew { get; set; }
        public VocabularyKey ItemClasses { get; set; }
        public VocabularyKey ItemIds { get; set; }
        public VocabularyKey LastDeliveryTime { get; set; }
        public VocabularyKey LastModifiedTime { get; set; }
        public VocabularyKey MessageCount { get; set; }
        public VocabularyKey Size { get; set; }
        public VocabularyKey Topic { get; set; }
        public VocabularyKey UniqueRecipients { get; set; }
        public VocabularyKey UniqueSenders { get; set; }
        public VocabularyKey UniqueUnreadSenders { get; set; }
        public VocabularyKey UnreadCount { get; set; }
    }
}
