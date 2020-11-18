using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.Crawling.Skype.Vocabularies
{
    public class MailHeadersVocabulary : DynamicVocabulary
    {
        public MailHeadersVocabulary()
        {
            VocabularyName = "Exchange Shared Mail Headers";
            KeyPrefix = "exchange.mail.headers";
            KeySeparator = "-";
            Grouping = EntityType.Unknown;
            ShowInApplication = false;
            ShowUrisInApplication = false;
        }
    }
}
