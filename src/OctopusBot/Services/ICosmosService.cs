using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using OctopusBot.ViewModels;

namespace OctopusBot.Services
{
    public interface ICosmosService : IDisposable
    {
        Task CreateDatabaseAsync(string databaseName);
        Task<IEnumerable<SynonymViewModel>> GetSynonymsAsync();
    }
}