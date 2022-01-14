using System.Collections.Generic;

namespace OctopusBot.Data
{
    public class TeamNameSynonymData
    {
        public readonly Dictionary<string, string> TeamNameDict = new()
        {
            ["arcus"] = "arcus",
            ["arc"] = "arcus",
            ["cloud"] = "cloud",
            ["tconnect"] = "tconnect",
            ["tconn"] = "tconnect",
            ["connect"] = "tconnect"

        };
        
    }
}