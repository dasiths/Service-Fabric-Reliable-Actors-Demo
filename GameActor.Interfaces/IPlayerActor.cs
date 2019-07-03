using System.Threading;
using System.Threading.Tasks;
using GameActor.Interfaces.Models;
using Microsoft.ServiceFabric.Actors;

namespace GameActor.Interfaces
{
    public interface IPlayerActor : IActor
    {
        Task InitializeAsync(string gameName, string playerName, CancellationToken cancellationToken);
        Task MoveToAsync(int x, int y, CancellationToken cancellationToken);
        Task<PlayerInfo> GetLatestInfoAsync(CancellationToken cancellationToken);
    }
}