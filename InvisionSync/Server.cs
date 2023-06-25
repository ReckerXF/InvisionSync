using CitizenFX.Core;
using CitizenFX.Core.Native;
using InvisionSync.Server.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static InvisionSync.Server.Models.Configuration;
using Debug = CitizenFX.Core.Debug;

namespace InvisionSync.Server
{
    public class Server : BaseScript
    {
        #region Variables
        internal static User currentUser = new User();
        internal static string deferralCardJson = "";
        #endregion

        #region Constructor
        public Server()
        {
            Debug.WriteLine("^5InvisionSync by Michael.#3080 loaded!^7");

            try
            {
                deferralCardJson = API.LoadResourceFile(API.GetCurrentResourceName(), "deferralCard.json");

                if (string.IsNullOrEmpty(deferralCardJson))
                {
                    Debug.WriteLine("The deferralCard.json file is empty!");
                }
            }

            catch (Exception e)
            {
                Debug.WriteLine($"Json Error Reported: {e.Message}\nStackTrace:\n{e.StackTrace}");
            }
        }
        #endregion

        #region Methods

        #region RetrieveAPI
        /// <summary>
        /// Retrieves the Invision API data as a Json, and runs a check to see if the hex on the forums matches their hex (optimized way of doing things).
        /// </summary>
        internal async Task RetrieveAPI(Player player)
        {
            try
            {
                // Handling the API Call
                var client = new HttpClient();
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                int pageNum = 1;
                int totalPages = 0;
                bool userFound = false;

                while (userFound == false && (pageNum <= totalPages || totalPages == 0))
                {
                    var uri = new UriBuilder($"{ConfigHandler.websiteURL}/api/core/members?perPage=25&key={ConfigHandler.APIKey}&page={pageNum}").Uri;
                    var response = await client.GetAsync(uri);
                    var result = JsonConvert.DeserializeObject<object>(response.Content.ReadAsStringAsync().Result).ToString();
                    
                    pageNum++;
                    totalPages = (int)JObject.Parse(result)["totalPages"];
                    
                    // Build the User.
                    foreach (JToken obj in JObject.Parse(result)["results"])
                    {
                        foreach (var w in obj["customFields"])
                        {
                            foreach (var x in w)
                            {
                                foreach (var y in x["fields"])
                                {
                                    foreach (var z in y)
                                    {
                                        if (z["name"].ToString() == "Steam Hex")
                                        {
                                            if (z["value"].ToString().ToLower() == player.Identifiers["steam"])
                                            {
                                                BuildUser(obj, z["value"].ToString());
                                                userFound = true;
                                                await Delay(200);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    await Delay(100);
                }
                
                await Delay(100);
            }
            
            catch (Exception e)
            {
                Debug.WriteLine($"^1Something has went wrong with fetching the API data... See Error: {e.Message}\nStackTrace:\n{e.StackTrace}^7");
                await Delay(100);
            }
        }
        #endregion

        #region BuildUser
        /// <summary>
        /// Builds the user data.
        /// </summary>
        /// <param name="apiResult"></param>
        /// <param name="retrievedHex"></param>
        /// <returns></returns>
        internal static async void BuildUser(JToken apiResult, string retrievedHex)
        {    
            try
            {
                currentUser.steamHex = retrievedHex;
                currentUser.primaryGroupId = int.Parse(apiResult["primaryGroup"]["id"].ToString());
                currentUser.forumName = apiResult["name"].ToString();

                List<int> secondaryGroups = new List<int>();

                foreach (var x in apiResult["secondaryGroups"])
                {
                    secondaryGroups.Add(int.Parse(x["id"].ToString()));
                }

                await Delay(200);

                currentUser.secondaryGroupIds = secondaryGroups;

                if (ConfigHandler.DebugMode)
                {
                    Debug.WriteLine("^1[DEBUG] User Built!");
                    Debug.WriteLine("====================================");
                    Debug.WriteLine("Forum Name: " + currentUser.forumName);
                    Debug.WriteLine("Steam Hex: " + currentUser.steamHex);
                    Debug.WriteLine("Primary Group Id: " + currentUser.primaryGroupId);
                    Debug.WriteLine("Secondary Group Ids: " + string.Join(", ", currentUser.secondaryGroupIds) + "^7");
                }
            }

            catch (Exception e)
            {
                Debug.WriteLine($"^5Something has went wrong while building the currentUser object... See Error: {e.Message}\nStackTrace:\n{e.StackTrace}^7");
            }
        }
        #endregion

        #region IsWhitelisted
        /// <summary>
        ///  Check if the player is whitelisted.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal static bool IsWhitelisted(Player player)
        {
            try
            {
                if (ConfigHandler.WhitelistedGroups.Contains(currentUser.primaryGroupId))
                {
                    return true;
                }
                else if (ConfigHandler.CheckSecondaryGroups)
                {
                    if (currentUser.secondaryGroupIds == null || currentUser.secondaryGroupIds.Count == 0)
                        return false;

                    if (ConfigHandler.WhitelistedGroups.Intersect(currentUser.secondaryGroupIds).Any())
                        return true;

                    return false;
                }

                return false;
            }

            catch (Exception e)
            {
                Debug.WriteLine($"^5Something has went wrong with checking if the player is whitelisted... See Error: {e.Message}\nStackTrace:\n{e.StackTrace}^7");
                return false;
            }
        }
        #endregion

        #region GetACEGroup
        /// <summary>
        ///  Gets the player's ACE group based on their Invision Group.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal static string GetACEGroup(Player player)
        {
            try
            {
                foreach (var perm in ConfigHandler.AceSyncGroups)
                {
                    if (perm.Value.Contains(currentUser.primaryGroupId))
                        return perm.Key;
                }
                    
                if (ConfigHandler.CheckSecondaryGroups)
                {
                    if (currentUser.secondaryGroupIds == null || currentUser.secondaryGroupIds.Count == 0)
                        return ConfigHandler.DefaultGroup;

                    foreach (var perm in ConfigHandler.AceSyncGroups)
                    {
                        if (currentUser.secondaryGroupIds.Intersect(perm.Value).Any())
                            return perm.Key;
                    }
                }

                return ConfigHandler.DefaultGroup;
            }

            catch (Exception e)
            {
                Debug.WriteLine($"^5Something has went wrong with getting the player's ACE group... See Error:{e.Message}\nStackTrace:\n{e.StackTrace}");
                return ConfigHandler.DefaultGroup;
            }
        }
        #endregion

        #region ResetData
        /// <summary>
        /// Resets the lists used after someone connects.
        /// </summary>
        internal static void ResetData()
        {
            currentUser = new User();
        }
        #endregion

        #endregion

        #region Events

        #region playerConnecting
        [EventHandler("playerConnecting")]
        private async void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            deferrals.defer();
            deferrals.update("Retrieving Forum API Data...");      
            
            if (player.Identifiers["steam"] == null || player.Identifiers["steam"] == "")
            {
                deferrals.done("You must be logged into Steam in order to join the server!");
                return;
            }

            await RetrieveAPI(player);

            deferrals.update("Checking whitelist status...");

            if (!IsWhitelisted(player) && ConfigHandler.WhitelistEnabled)
            {
                ResetData();
                deferrals.presentCard(deferralCardJson);
                //deferrals.done(ConfigHandler.UnwhitelistedMessage);
            }
            else if (ConfigHandler.AceSyncEnabled)
            {
                var hex = currentUser.steamHex;
                var group = GetACEGroup(player);

                // Set ACE Permissions
                Debug.WriteLine($"{player.Name} ({player.Identifiers["steam"]}) has joined the server with ACE permissions: {group}");
                API.ExecuteCommand($"add_principal identifier.steam:{hex} {group}");

                ResetData();
                deferrals.done();
            }
            else
            {
                ResetData();
                deferrals.done();
            }
        }
        #endregion

        #region playerDropped
        [EventHandler("playerDropped")]
        private static void OnPlayerDropped([FromSource] Player player, string reason)
        {
            // Reset ACE permissions
            Debug.WriteLine($"{player.Name} has left the server. Reason: {reason}");
            API.ExecuteCommand($"add_principal steam.identifier:{player.Identifiers["steam"]} {ConfigHandler.DefaultGroup}");
        }
        #endregion

        #endregion
    }
}
