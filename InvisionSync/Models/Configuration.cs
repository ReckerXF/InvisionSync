using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvisionSync.Server.Models
{
    internal class Configuration
    {
        public struct Config
        {
            public string websiteURL { get; set; }
            public string apiKey { get; set; }
            public bool debugMode { get; set; }
            public InvisionWL invisionWhitelist { get; set; }
            public AceSync aceSync { get; set; }
        }

        public struct InvisionWL
        {
            public bool enabled { get; set; }
            public int[] whitelistedGroups { get; set; }
            public string unwhitelistedMessage { get; set; }
        }

        public struct AceSync
        {
            public bool enabled { get; set; }
            public string defaultGroup { get; set; }
            public Dictionary<string, int[]> groups { get; set; }
            public bool checkSecondaryGroups { get; set; }
        }
    }
}
