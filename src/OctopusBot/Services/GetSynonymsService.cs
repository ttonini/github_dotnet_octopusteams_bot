using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using OctopusBot.ViewModels;

namespace OctopusBot.Services
{
    public class GetSynonymsService : IGetSynonymsService
    {
        private readonly IOptions<AppSettings> _settings;

        public GetSynonymsService(IOptions<AppSettings> settings)
        {
            _settings = settings;
        }


        public List<Dictionary<string, string>> GetSynonymsDictionaryList(IEnumerable<SynonymViewModel> data)
        {
            Dictionary<string, string > EnvironmentDict = new Dictionary<string, string>();
            Dictionary<string, string > ServiceDict = new Dictionary<string, string>();
            Dictionary<string, string > LanguageDict = new Dictionary<string, string>();
            Dictionary<string, string > TeamDict = new Dictionary<string, string>();

            foreach(SynonymViewModel model in data)
            {
                if(model.PartitionKey == "Environment"){
                    if(!EnvironmentDict.ContainsKey(model.Key.ToLower())){
                        EnvironmentDict.Add(model.Key.ToLower(), model.Value.ToLower());
                    }
                }
                if(model.PartitionKey == "Service"){
                    if(!ServiceDict.ContainsKey(model.Key.ToLower())){
                        ServiceDict.Add(model.Key.ToLower(), model.Value.ToLower());
                    }
                }
                if(model.PartitionKey == "Team"){
                    if(!LanguageDict.ContainsKey(model.Key.ToLower())){
                        TeamDict.Add(model.Key.ToLower(), model.Value.ToLower());
                    }
                }
                if(model.PartitionKey == "Language"){
                    if(!TeamDict.ContainsKey(model.Key.ToLower())){
                        LanguageDict.Add(model.Key.ToLower(), model.Value.ToLower());
                    }
                }
            } 

            List<Dictionary<string, string>> SynonymsDict = new List<Dictionary<string, string>> { ServiceDict, TeamDict, LanguageDict, EnvironmentDict };

            return SynonymsDict;
        }
        
    }
}