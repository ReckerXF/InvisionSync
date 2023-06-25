using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static InvisionSync.Server.Models.Configuration;

namespace InvisionSync.Server
{
    internal class ConfigHandler : BaseScript
    {
        #region Config Variables
        /// <summary>
        /// Obtain the website URL
        /// </summary>
        public static string websiteURL = GetConfig().websiteURL;

        /// <summary>
        /// Obtain the API Key
        /// </summary>
        public static string APIKey => GetConfig().apiKey;

        /// <summary>
        /// Debug mode enabled?
        /// </summary>
        public static bool DebugMode => GetConfig().debugMode;

        /// <summary>
        /// Is the whitelist enabled?
        /// </summary>
        public static bool WhitelistEnabled => GetConfig().invisionWhitelist.enabled;

        /// <summary>
        /// Unauthorized group Ids
        /// </summary>
        internal static int[] WhitelistedGroups => GetConfig().invisionWhitelist.whitelistedGroups;

        /// <summary>
        /// Enable aceSync?
        /// </summary>
        internal static bool AceSyncEnabled => GetConfig().aceSync.enabled;

        /// <summary>
        /// Default group
        /// </summary>
        internal static string DefaultGroup => GetConfig().aceSync.defaultGroup;

        /// <summary>
        /// Obtain the groups
        /// </summary>
        internal static Dictionary<string, int[]> AceSyncGroups => GetConfig().aceSync.groups;

        /// <summary>
        /// Obtain the secondary groups
        /// </summary>
        internal static bool CheckSecondaryGroups => GetConfig().aceSync.checkSecondaryGroups;
        #endregion

        #region Decoding config.json
        public static Config GetConfig()
        {
            Config data = new Config();
            string jsonFile = API.LoadResourceFile(API.GetCurrentResourceName(), "config.json");

            try
            {
                if (string.IsNullOrEmpty(jsonFile))
                {
                    System.Diagnostics.Debug.WriteLine("The config.json file is empty!");
                }
                else
                {
                    data = JsonConvert.DeserializeObject<Config>(jsonFile);
                }
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Json Error Reported: {e.Message}\nStackTrace:\n{e.StackTrace}");
            }

            return data;
        }
        #endregion
    }
}
