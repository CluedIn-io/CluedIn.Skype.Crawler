using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CluedIn.Core.Logging;
using CluedIn.Core.Providers;
using CluedIn.Crawling.Skype.Core;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;
using RestSharp;

namespace CluedIn.Crawling.Skype.Infrastructure
{
    // TODO - This class should act as a client to retrieve the data to be crawled.
    // It should provide the appropriate methods to get the data
    // according to the type of data source (e.g. for AD, GetUsers, GetRoles, etc.)
    // It can receive a IRestClient as a dependency to talk to a RestAPI endpoint.
    // This class should not contain crawling logic (i.e. in which order things are retrieved)
    public class SkypeClient
    {
        private const string BaseUri = "http://sample.com";

        private readonly ILogger log;

        private readonly IRestClient client;

        ExchangeService _exchangeService = null;
        int _pageSize = 50;
        Folder _imHistoryFolder = null;
        List<EmailMessage> _imHistory = null;

        public SkypeClient(ILogger log, SkypeCrawlJobData skypeCrawlJobData, IRestClient client) // TODO: pass on any extra dependencies
        {
            if (skypeCrawlJobData == null)
            {
                throw new ArgumentNullException(nameof(skypeCrawlJobData));
            }

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _exchangeService = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
            _exchangeService.UseDefaultCredentials = false;
            _exchangeService.Credentials = new WebCredentials(skypeCrawlJobData.email, skypeCrawlJobData.password);
            _exchangeService.AutodiscoverUrl(skypeCrawlJobData.email);

            _imHistory = new List<EmailMessage>();

            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.client = client ?? throw new ArgumentNullException(nameof(client));

            // TODO use info from skypeCrawlJobData to instantiate the connection
            client.BaseUrl = new Uri(BaseUri);
            client.AddDefaultParameter("api_key", skypeCrawlJobData.ApiKey, ParameterType.QueryString);
        }

        private async Task<T> GetAsync<T>(string url)
        {
            var request = new RestRequest(url, Method.GET);

            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var diagnosticMessage = $"Request to {client.BaseUrl}{url} failed, response {response.ErrorMessage} ({response.StatusCode})";
                log.Error(() => diagnosticMessage);
                throw new InvalidOperationException($"Communication to jsonplaceholder unavailable. {diagnosticMessage}");
            }

            var data = JsonConvert.DeserializeObject<T>(response.Content);

            return data;
        }

        public AccountInformation GetAccountInformation()
        {
            //TODO - return some unique information about the remote data source
            // that uniquely identifies the account
            return new AccountInformation("", ""); 
        }

        public IEnumerable<Item> Get()
        {
            // Get the "Conversation History" folder, if not already found.
            if (_imHistoryFolder == null)
            {
                _imHistoryFolder = this.FindImHistoryFolder();
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
                findResults = _exchangeService.FindItems(_imHistoryFolder.Id, searchFilterCollection, itemView);
                imHistoryItems.AddRange(findResults);
                itemView.Offset += _pageSize;
            } while (findResults.MoreAvailable);

            foreach (var item in imHistoryItems)
            {
                yield return item;
            }
        }

        private Folder FindImHistoryFolder()
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
                findFolderResults = _exchangeService.FindFolders(WellKnownFolderName.MsgFolderRoot, folderView);
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
    }
}
