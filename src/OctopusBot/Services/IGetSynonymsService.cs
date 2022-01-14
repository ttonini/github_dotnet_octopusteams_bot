using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using OctopusBot.ViewModels;

namespace OctopusBot.Services
{
    public interface IGetSynonymsService
    {
        List<Dictionary<string, string>> GetSynonymsDictionaryList(IEnumerable<SynonymViewModel> data);

    }
}