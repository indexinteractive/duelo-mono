namespace Duelo.Gameboard.MapDecorator
{
    using Duelo.Common.Pathfinding;

    public interface IMapDecorator
    {
        public void PaintPathTiles(Path<MapTile> path);
    }
}