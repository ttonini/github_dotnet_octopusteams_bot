using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OctopusBot.ViewModels;

namespace OctopusBot.Services
{
    public class CosmosService : ICosmosService
    {

        private Container _container;
        private CosmosClient _cosmosClient;
        private int _maxItemCount;

        public CosmosService(IOptions<AppSettings> appSettings)
        {
            _cosmosClient = new CosmosClient(appSettings.Value.CosmosDbConnection.EndpointUri, appSettings.Value.CosmosDbConnection.PrimaryKey);
            _maxItemCount = appSettings.Value.CosmosDbConnection.MaxItemCount;
            CreateDatabaseAsync(appSettings.Value.CosmosDbConnection.DatabaseName).Wait();
            // TODO: refactor "Containers" to better name
            this._container = _cosmosClient.GetContainer(appSettings.Value.CosmosDbConnection.DatabaseName, appSettings.Value.CosmosDbConnection.Containers);
        }

        public IConfiguration Configuration { get; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task CreateDatabaseAsync(string databaseName)
        {
            Database db = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            Console.WriteLine("Created Database: {0}\n", db.Id);
        }

        public async Task<IEnumerable<SynonymViewModel>> GetSynonymsAsync()
        {
            //get the data from all the containers
            var iterator = _container
                .GetItemLinqQueryable<SynonymViewModel>(false, null, new QueryRequestOptions { MaxItemCount = _maxItemCount })
                .ToFeedIterator();

            //getitemfeediterator
            //loop through and add to the results list which is a SynonymViewModel until there are no more elements
            var results = await iterator.ReadNextAsync();
            var contents = results.Resource;

            return contents;
        }
    }
}