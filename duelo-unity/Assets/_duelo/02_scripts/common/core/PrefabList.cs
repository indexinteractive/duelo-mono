namespace Duelo.Common.Core
{
    using System.Collections.Generic;
    using Duelo.Common.Player;
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

    /// <summary>
    /// Prefab list stores all prefabs that are instantiated at runtime by name.
    /// Each map is built from a list that can be assigned to from the editor, since
    /// unity cannot handle a dictionary in the editor
    /// </summary>
    public class PrefabList : MonoBehaviour
    {
        #region Unity Lists
        [SerializeField]
        private PrefabEntry[] _tileList;
        [SerializeField]
        private PrefabEntry[] _uiList;
        [SerializeField]
        private GameObject[] _characterList;
        #endregion

        #region Lookups
        public Dictionary<string, PrefabEntry> TileLookup = new();
        public Dictionary<string, GameObject> MenuLookup = new();
        public Dictionary<string, GameObject> CharacterLookup = new();
        #endregion

        #region Unity Lifecycle
        public void Start()
        {
            foreach (PrefabEntry entry in _tileList)
            {
                TileLookup[entry.name] = entry;
            }

            foreach (PrefabEntry entry in _uiList)
            {
                MenuLookup[entry.name] = entry.prefab;
            }

            foreach (GameObject entry in _characterList)
            {
                var traits = entry.GetComponent<PlayerTraits>();
                if (CharacterLookup.ContainsKey(traits.CharacterId))
                {
                    Debug.LogError($"[PrefabList] Duplicate character id: {traits.CharacterId}");
                    Application.Quit(Duelo.Common.Util.ExitCode.DuplicatePrefab);
                }

                CharacterLookup[traits.CharacterId] = entry;
            }
        }
        #endregion
    }
}