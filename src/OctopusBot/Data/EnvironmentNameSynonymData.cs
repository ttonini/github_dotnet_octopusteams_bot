using System.Collections.Generic;

namespace OctopusBot.Data
{
    public class EnvironmentNameSynonymData
    {
        public readonly Dictionary<string, string> EnvironmentNameDict = new()
        {
            ["dev"] = "dev",
            ["develop"] = "dev",
            ["development"] = "dev",
            ["clinical"] = "clinical-trial",
            ["clinical trial"] = "clinical-trial",
            ["clinical-trial"] = "clinical-trial",
            ["clinical_trial"] = "clinical-trial",
            ["trial"] = "clinical-trial",
            ["prod"] = "production-blue",
            ["production"] = "production-blue",
            ["production blue"] = "production-blue",
            ["prod blue"] = "production-blue",
            ["production_blue"] = "production-blue",
            ["production-blue"] = "production-blue",
            ["blue"] = "test-blue",
            ["test blue"] = "test-blue",
            ["test_blue"] = "test-blue",
            ["test-blue"] = "test-blue",
            ["green"] = "test-green",
            ["test green"] = "test-green",
            ["test_green"] = "test-green",
            ["test-green"] = "test-green",
            ["sit"] = "sit"

        };

    }
}