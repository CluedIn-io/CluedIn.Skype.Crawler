using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.Crawling.Skype.Vocabularies
{
    public class ExchangeSharedMailboxExtendedPropertiesVocabulary : DynamicVocabulary
    {
        public ExchangeSharedMailboxExtendedPropertiesVocabulary()
        {
            this.VocabularyName = "Exchange Shared Mail Extended Properties";
            this.KeyPrefix = "exchange.mail.extendedProperties";
            this.KeySeparator = "-";
            this.Grouping = EntityType.Unknown;
            this.ShowInApplication = false;
            this.ShowUrisInApplication = false;
        }
    }
}
