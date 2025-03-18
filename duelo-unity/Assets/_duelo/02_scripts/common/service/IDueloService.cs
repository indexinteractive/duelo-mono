namespace Duelo.Common.Service
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Gameboard;

    public interface IDueloService
    {
        public UniTask<MatchDto> GetMatch(string matchId);
        public UniTask<DueloPlayerDto> GetDevicePlayer();
        public UniTask<DueloMapDto> GetMap(string mapId);
        // public UniTask SaveMap(string mapId, DueloMapDto map);
        public UniTask<DueloPlayerDto> GetPlayerById(string playerId);
        public UniTask SetActiveProfile(string playerId, string profileId);
        public UniTask<PlayerProfileDto> CreateProfile(string playerId, string gamertag, string characterId);
    }
}