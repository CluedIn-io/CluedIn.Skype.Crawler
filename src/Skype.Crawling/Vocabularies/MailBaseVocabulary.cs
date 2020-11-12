using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.Crawling.Skype.Vocabularies
{
    public abstract class MailBaseVocabulary : ItemVocabulary
    {
        protected MailBaseVocabulary()
        {
            AddGroup("Exchange Mail Details", group =>
            {
                From = group.Add(new VocabularyKey("from"));
                Sender = group.Add(new VocabularyKey("sender"));
                ReceivedBy = group.Add(new VocabularyKey("receivedBy"));
                ReceivedRepresenting = group.Add(new VocabularyKey("receivedRepresenting"));
                To = group.Add(new VocabularyKey("to"));
                Cc = group.Add(new VocabularyKey("cc"));
                Bcc = group.Add(new VocabularyKey("bcc"));
                ReplyTo = group.Add(new VocabularyKey("replyTo"));

                IsRead = group.Add(new VocabularyKey("isRead", VocabularyKeyDataType.Boolean));
                InternetMessageId = group.Add(new VocabularyKey("internetMessageId"));
                ConversationTopic = group.Add(new VocabularyKey("conversationTopic"));
                IsDeliveryReceiptRequested = group.Add(new VocabularyKey("isDeliveryReceiptRequested", VocabularyKeyDataType.Boolean));
                IsResponseRequested = group.Add(new VocabularyKey("isResponseRequested", VocabularyKeyDataType.Boolean));
                References = group.Add(new VocabularyKey("references"));
            });
        }

        public VocabularyKey From { get; internal set; }
        public VocabularyKey Sender { get; internal set; }
        public VocabularyKey To { get; internal set; }
        public VocabularyKey Cc { get; internal set; }
        public VocabularyKey Bcc { get; internal set; }

        public VocabularyKey ReplyTo { get; internal set; }
        public VocabularyKey ReceivedBy { get; internal set; }
        public VocabularyKey ReceivedRepresenting { get; internal set; }

        public VocabularyKey IsRead { get; internal set; }
        public VocabularyKey InternetMessageId { get; internal set; }
        public VocabularyKey ConversationTopic { get; internal set; }
        public VocabularyKey IsDeliveryReceiptRequested { get; internal set; }
        public VocabularyKey IsResponseRequested { get; internal set; }
        public VocabularyKey References { get; internal set; }
    }
}
