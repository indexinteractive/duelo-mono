using Duelo.Common.Model;

namespace Duelo.Client.Match
{
    public class ClientMatch
    {
        private readonly MatchDto _matchDto;

        public ClientMatch(MatchDto matchDto)
        {
            _matchDto = matchDto;
        }
    }
}