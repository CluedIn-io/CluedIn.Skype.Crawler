using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.Crawling.Skype.Vocabularies
{
    public class FlagVocabulary : SimpleVocabulary
    {
        public FlagVocabulary()
        {
            VocabularyName = "Exchange Item Flag";
            KeyPrefix = "exchange.item.flag";
            KeySeparator = ".";
            Grouping = EntityType.Unknown;

            AddGroup("Exchange Item Flag", group =>
            {
                CompleteDate = group.Add(new VocabularyKey("completeDate", VocabularyKeyDataType.DateTime));
                DueDate = group.Add(new VocabularyKey("dueDate", VocabularyKeyDataType.DateTime));
                FlagStatus = group.Add(new VocabularyKey("flagStatus"));
                StartDate = group.Add(new VocabularyKey("startDate", VocabularyKeyDataType.DateTime));
            });
        }

        public VocabularyKey CompleteDate { get; internal set; }
        public VocabularyKey DueDate { get; internal set; }
        public VocabularyKey FlagStatus { get; internal set; }
        public VocabularyKey StartDate { get; internal set; }
    }
}
