namespace Duelo.Gameboard.MapDecorator
{
    using System.Collections.Generic;
    using Duelo.Common.Model;

    public interface IMapDecorator
    {
        #region Player Movement Path
        public void PaintPathTiles(PlayerRole role, List<MapTile> path);
        public void ClearPath(PlayerRole role);
        #endregion

        #region Movable Tiles / ChooseMovementPhase
        public void PaintMovableTiles(List<MapTile> tiles);
        public void ClearMovableTiles();
        #endregion

        #region Player Action Tiles / ChooseActionPhase
        public void PaintActionTiles(List<MapTile> tiles);
        public void ClearActionTiles();
        #endregion
    }
}