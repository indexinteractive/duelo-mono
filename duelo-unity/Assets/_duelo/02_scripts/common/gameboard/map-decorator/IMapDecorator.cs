namespace Duelo.Gameboard.MapDecorator
{
    using System.Collections.Generic;

    public interface IMapDecorator
    {
        public void PaintPathTiles(List<MapTile> path);
        public void PaintMovableTiles(List<MapTile> tiles);
        public void ClearMovableTiles();
    }
}