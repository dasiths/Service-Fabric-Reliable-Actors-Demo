using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameActor.Interfaces.Models;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace GameActor.Interfaces
{
    public interface IGameActor : IActor, IActorEventPublisher<IGameEvents>
    {
        Task<int> GetCountAsync(CancellationToken cancellationToken);
        Task SetCountAsync(int count, CancellationToken cancellationToken);
        Task<string> JoinGameAsync(string playerName, CancellationToken cancellationToken);
        Task<IList<PlayerInfo>> GetLatestPlayerInfoAsync(CancellationToken cancellationToken);
    }
}
