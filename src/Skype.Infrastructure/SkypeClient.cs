using System;
using System.Net;
using System.Threading.Tasks;
using CluedIn.Core.Providers;
using CluedIn.Crawling.Skype.Core;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Exchange.WebServices.Data;
using System.Collections.Generic;

namespace CluedIn.Crawling.Skype.Infrastructure
{
    public class SkypeClient
    {
        ExchangeService _exchangeService = null;
        int _pageSize = 50;
        Folder _imHistoryFolder = null;
        SkypeCrawlJobData _crawlJobData;

        public SkypeClient(SkypeCrawlJobData skypeCrawlJobData)
        {
            if (skypeCrawlJobData == null)
            {
                throw new ArgumentNullException(nameof(skypeCrawlJobData));
            }

            _crawlJobData = skypeCrawlJobData;
            _exchangeService = new ExchangeService(ExchangeVersion.Exchange2016);
            _exchangeService.UseDefaultCredentials = false;
            _exchangeService.Credentials = new WebCredentials(skypeCrawlJobData.Email, skypeCrawlJobData.Password);
            _exchangeService.AutodiscoverUrl(skypeCrawlJobData.Email, ValidationCallback);
        }

        private static bool ValidationCallback(string redirectionUrl)
        {
            bool result = false;

            var redirectionUri = new Uri(redirectionUrl);

            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }

            return result;
        }

        public IEnumerable<Item> Get()
        {
            return GetItemsAsync().Result;
        }

        private async Task<IEnumerable<Item>> GetItemsAsync()
        {
            // Get the "Conversation History" folder, if not already found.
            if (_imHistoryFolder == null)
            {
                _imHistoryFolder = await this.FindImHistoryFolder();
                if (_imHistoryFolder == null)
                {
                    throw new Exception("Could not find history folder");
                }
            }

            // Get Conversation History items.
            List<Item> imHistoryItems = new List<Item>();
            FindItemsResults<Item> findResults;

            ItemView itemView = new ItemView(_pageSize);
            itemView.PropertySet = new PropertySet(BasePropertySet.IdOnly);
            SearchFilter.SearchFilterCollection searchFilterCollection = null;

            do
            {
                findResults = await _exchangeService.FindItems(_imHistoryFolder.Id, searchFilterCollection, itemView);
                imHistoryItems.AddRange(findResults);
                itemView.Offset += _pageSize;
            } while (findResults.MoreAvailable);

            return imHistoryItems;
        }

        private async Task<Folder> FindImHistoryFolder()
        {
            FolderView folderView = new FolderView(_pageSize, 0);
            folderView.PropertySet = new PropertySet(BasePropertySet.IdOnly);
            folderView.PropertySet.Add(FolderSchema.DisplayName);
            folderView.PropertySet.Add(FolderSchema.ChildFolderCount);

            folderView.Traversal = FolderTraversal.Shallow;
            Folder imHistoryFolder = null;

            FindFoldersResults findFolderResults;
            bool foundImHistoryFolder = false;
            do
            {
                findFolderResults = await _exchangeService.FindFolders(WellKnownFolderName.MsgFolderRoot, folderView);
                foreach (Folder folder in findFolderResults)
                {
                    if (folder.DisplayName.ToLower() == "conversation history")
                    {
                        imHistoryFolder = folder;
                        foundImHistoryFolder = true;
                    }
                }
                folderView.Offset += _pageSize;
            } while (findFolderResults.MoreAvailable && !foundImHistoryFolder);

            return imHistoryFolder;
        }

        public AccountInformation GetAccountInformation()
        {
            return new AccountInformation(_crawlJobData.Email, _crawlJobData.Email);
        }

    }
}
