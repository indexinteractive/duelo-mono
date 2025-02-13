namespace Duelo.Common.Service
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Gameboard;
    using UnityEngine;

    public class MockService
    {
        private readonly MatchDto _matchDto;
        private readonly DueloPlayerDto _devicePlayer;

        public MockService(MatchDto matchDto)
        {
            _matchDto = matchDto;
            var deviceMatchPlayer = _matchDto.Players.Challenger;

            var profiles = new Dictionary<string, PlayerProfileDto>();
            profiles.Add(deviceMatchPlayer.UnityPlayerId, deviceMatchPlayer.Profile);

            _devicePlayer = new DueloPlayerDto()
            {
                UnityPlayerId = deviceMatchPlayer.UnityPlayerId,
                Profiles = profiles,
                ActiveProfileId = deviceMatchPlayer.Profile.Id
            };
        }

        public UniTask<DueloPlayerDto> GetDevicePlayer() => UniTask.FromResult(_devicePlayer);

        public UniTask<DueloMapDto> GetMap(string mapId)
        {
            void setTileAtIndex(List<GridTileDto> tiles, int x, int z, string type)
            {
                int index = z * 12 + x;
                tiles[index].Type = type;
            }

            int mapSize = 12;
            int defaultZ = 0;
            DueloMapDto debugMap = new DueloMapDto()
            {
                DecoratorClass = "BasicMapDecorator",
                Name = mapId
            };

            debugMap.Tiles = new List<GridTileDto>(mapSize * mapSize);

            for (int x = 0; x < mapSize; x++)
            {
                for (int z = 0; z < mapSize; z++)
                {
                    var tile = new GridTileDto();
                    tile.Position = new Vector3(x, defaultZ, z);
                    tile.Type = "devtile";

                    debugMap.Tiles.Add(tile);
                }
            }

            setTileAtIndex(debugMap.Tiles, Mathf.FloorToInt(mapSize / 2) - 1, mapSize - 1, "spawn_challenger");
            setTileAtIndex(debugMap.Tiles, Mathf.CeilToInt(mapSize / 2), 0, "spawn_defender");

            return UniTask.FromResult(debugMap);
        }
    }
}