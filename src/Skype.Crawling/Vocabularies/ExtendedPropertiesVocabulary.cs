using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.Crawling.Skype.Vocabularies
{
    public class ExtendedPropertiesVocabulary : DynamicVocabulary
    {
        public ExtendedPropertiesVocabulary()
        {
            VocabularyName = "Exchange Shared Mail Extended Properties";
            KeyPrefix = "exchange.mail.extendedProperties";
            KeySeparator = "-";
            Grouping = EntityType.Unknown;
            ShowInApplication = false;
            ShowUrisInApplication = false;
        }
    }
}
