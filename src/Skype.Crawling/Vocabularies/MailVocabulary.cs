using CluedIn.Core.Data;

namespace CluedIn.Crawling.Skype.Vocabularies
{
    public class MailVocabulary : MailBaseVocabulary
    {
        public MailVocabulary()
        {
            VocabularyName = "Exchange Mail";
            KeyPrefix = "exchange.mail";
            KeySeparator = ".";
            Grouping = EntityType.Mail;

            AddMapping(HasAttachments, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInMail.HasAttachments);
            AddMapping(From, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInMail.From);
            AddMapping(To, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInMail.To);
            AddMapping(Bcc, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInMail.Bcc);
            AddMapping(Cc, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInMail.Cc);
            AddMapping(Body, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInMail.Body);
            AddMapping(DateTimeSent, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInMail.SentDate);
            AddMapping(DateTimeReceived, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInMail.ReceivedDate);
        }
    }
}
