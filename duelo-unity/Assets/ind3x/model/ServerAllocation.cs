namespace Ind3x.Model
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine;

    [Serializable]
    public class ServerAllocation
    {
        [JsonProperty("ipv6")]
        public string Ipv6;

        [JsonProperty("port")]
        public string Port;

        [JsonProperty("allocatedUUID")]
        public string AllocatedUuid;

        [JsonProperty("serverId")]
        public string ServerId;

        [JsonProperty("machineID")]
        public string MachineId;

        [JsonProperty("fleetID")]
        public string FleetId;

        [JsonProperty("regionID")]
        public string RegionId;

        [JsonProperty("regionName")]
        public string RegionName;

        [JsonProperty("ip")]
        public string Ip;

        [JsonProperty("queryPort")]
        public string QueryPort;

        [JsonProperty("queryType")]
        public string QueryType;

        [JsonProperty("serverLogDir")]
        public string ServerLogDir;

        #region Static Methods
        public static ServerAllocation ReadServerJson()
        {
            var serverJsonPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "server.json");
            Debug.Log("[ServerAllocation] Reading server.json from: " + serverJsonPath);

            if (!System.IO.File.Exists(serverJsonPath))
            {
                Debug.LogWarning("[ServerAllocation] server.json not found");
                return null;
            }

            var rawData = System.IO.File.ReadAllText(serverJsonPath);
            Debug.Log("[ServerAllocation] found server.json: " + rawData);

            return JsonConvert.DeserializeObject<Ind3x.Model.ServerAllocation>(rawData);
        }
        #endregion
    }
}