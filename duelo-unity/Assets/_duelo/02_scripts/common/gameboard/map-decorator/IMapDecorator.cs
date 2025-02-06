namespace Duelo.Gameboard.MapDecorator
{
    using System.Collections.Generic;
    using Duelo.Common.Model;

    public interface IMapDecorator
    {
        public void PaintPathTiles(PlayerRole role, List<MapTile> path);
        public void ClearPath(PlayerRole role);

        public void PaintMovableTiles(List<MapTile> tiles);
        public void ClearMovableTiles();
    }
}