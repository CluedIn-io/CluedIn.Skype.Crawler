// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeSharedMailboxMailHeadersVocabulary.cs" company="Clued In">
//   Copyright (c) 2018 Clued In. All rights reserved.
// </copyright>
// <summary>
//   Implements the exchange shared mailbox mail headers vocabulary class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
