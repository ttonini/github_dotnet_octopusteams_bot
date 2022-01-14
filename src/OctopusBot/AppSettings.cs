using System.Collections.Generic;

namespace OctopusBot
{
    public class AppSettings
    {
        public OctopusConnection OctopusConnection { get; set; }
        public SlugStringSpecialCases SlugStringSpecialCases { get; set; }
        public CosmosDbConnection CosmosDbConnection { get; set; }
        public Authentication Authentication { get; set; }
    }

    public class OctopusConnection
    {
        public string OctoUrl { get; set; }
        public string ApiKey { get; set; }
    }

    public class SlugStringSpecialCases
    {
        public List<string> NoTeamNameRequired { get; set; }
        public List<string> NoLanguageNameRequired { get; set; }
    }

    public class CosmosDbConnection
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseName { get; set; }
        public string Containers { get; set; }
        public string PartitionKey { get; set; }
        public int MaxItemCount { get; set; }
    }

    public class Authentication
    {
        public string ConnectionName { get; set; }
    }
}
