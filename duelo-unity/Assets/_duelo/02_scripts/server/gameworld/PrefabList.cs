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
        /// All prefabs in the game.
        /// These will be used to populate <see cref="PrefabLookup"/> , and will not affect
        /// anything if modified at runtime.
        /// </summary>
        public PrefabEntry[] AllPrefabs;

        [HideInInspector]
        public Dictionary<string, PrefabEntry> PrefabLookup;
        #endregion

        #region Unity Lifecycle
        public void Start()
        {
            PrefabLookup = new Dictionary<string, PrefabEntry>();
            foreach (PrefabEntry entry in AllPrefabs)
            {
                PrefabLookup[entry.name] = entry;
            }
        }
        #endregion
    }
}