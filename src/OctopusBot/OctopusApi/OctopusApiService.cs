using System;
using System.Collections.Generic;
using OctopusBot.ViewModels;

namespace OctopusBot.OctopusApi
{
    public class OctopusApiService
    {
        public readonly Dictionary<string, string> OctopusApiKeyServiceDict = new()
        {
            ["dev"] = "Environments-1",
            ["test-blue"] = "Environments-21",
            ["test-green"] = "Environments-61",
            ["sit"] = "Environments-82",
            ["production-tconnect"] = "Environments-324",
            ["production-blue"] = "Environments-42",
            ["production-green-inactive"] = "Environments-41",
            ["clinical-trial"] = "Environments-323"

        };
    }
}