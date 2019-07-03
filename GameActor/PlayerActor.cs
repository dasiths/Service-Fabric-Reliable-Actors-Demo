using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameActor.Interfaces;
using GameActor.Interfaces.Models;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace GameActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class PlayerActor : Actor, IPlayerActor
    {
        public PlayerActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            await this.StateManager.TryAddStateAsync("positions", new List<PlayerInfo>());
        }

        public async Task InitializeAsync(string gameName, string playerName, CancellationToken cancellationToken)
        {
            await this.StateManager.TryAddStateAsync("gameName", gameName, cancellationToken);
            await this.StateManager.TryAddStateAsync("playerName", playerName, cancellationToken);
        }

        public async Task MoveToAsync(int x, int y, CancellationToken cancellationToken)
        {
            var playerName = await StateManager.GetStateAsync<string>("playerName", cancellationToken);
            var playerInfo = new PlayerInfo()
            {
                LastUpdate = DateTimeOffset.Now,
                PlayerName = playerName,
                XCoordinate = x,
                YCoordinate = y
            };

            await StateManager.AddOrUpdateStateAsync("positions", new List<PlayerInfo>() { playerInfo }, (key, value) =>
            {
                value.Add(playerInfo);
                return value;
            }, cancellationToken);
        }

        public async Task<PlayerInfo> GetLatestInfoAsync(CancellationToken cancellationToken)
        {
            var positions = await StateManager.GetStateAsync<List<PlayerInfo>>("positions", cancellationToken);
            return positions.Last();
        }
    }
}