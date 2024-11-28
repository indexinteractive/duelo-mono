using Duelo.Common.Model;
using UnityEngine;

namespace Duelo.Gameboard.Tile
{
    public class PlayerSpawnTile : MapTile
    {
        #region Public Properties
        public PlayerRole Role;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            var material = GetComponentInChildren<Renderer>().material;

            Color color = Role == PlayerRole.Challenger
                ? new Color(1.0f, 0.5f, 0.0f, 0.75f)
                : new Color(0.0f, 1.0f, 0.0f, 0.75f);

            material.color = color;
            material.SetColor("_EmissionColor", color);
        }
        #endregion
    }
}