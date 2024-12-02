namespace Duelo.Server.GameWorld
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public struct PrefabEntry
    {
        #region Fields
        public string name;
        public GameObject prefab;
        public int maxCount;
        #endregion

        #region Initialization
        public PrefabEntry(string name, GameObject prefab, int maxCount)
        {
            this.name = name;
            this.prefab = prefab;
            this.maxCount = maxCount;
        }
        #endregion
    }

    public class PrefabList : MonoBehaviour
    {
        #region Public Properties
        /// <summary>
        /// These will be used to populate <see cref="TileLookup"/> , and will not affect
        /// anything if modified at runtime.
        /// </summary>
        public PrefabEntry[] Tiles;

        [HideInInspector]
        public Dictionary<string, PrefabEntry> TileLookup = new();
        #endregion

        #region Unity Lifecycle
        public void Start()
        {
            foreach (PrefabEntry entry in Tiles)
            {
                TileLookup[entry.name] = entry;
            }
        }
        #endregion
    }
}