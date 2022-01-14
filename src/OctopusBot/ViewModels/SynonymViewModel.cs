using System;
using System.Text.Json.Serialization;

namespace OctopusBot.ViewModels
{
    public class SynonymViewModel
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("partitionKey")]
        public string PartitionKey { get; set; }
    }
}
