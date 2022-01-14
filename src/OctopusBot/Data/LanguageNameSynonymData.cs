using System.Collections.Generic;

namespace OctopusBot.Data
{
    public class LanguageNameSynonymData
    {
        public readonly Dictionary<string, string> LanguageNameDict = new()
        {
            ["dotnet"] = "dotnet",
            [".net"] = "dotnet",
            ["dot net"] = "dotnet",
            ["c#"] = "dotnet",
            ["node"] = "node",
            ["js"] = "node",
            ["javascript"] = "node"

        };
        
    }
}