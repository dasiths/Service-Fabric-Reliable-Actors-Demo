using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using GameActor.Interfaces;
using GameActor.Interfaces.Models;

namespace GameActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class GameActor : Actor, IGameActor
    {
        private const string PlayerActorUri = "fabric:/ReliableActorsDemo/PlayerActorService";

        /// <summary>
        /// Initializes a new instance of GameActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public GameActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            await this.StateManager.TryAddStateAsync("count", 0);
            await this.StateManager.TryAddStateAsync("players", new List<string>());
        }

        public async Task<string> JoinGameAsync(string playerName, CancellationToken cancellationToken)
        {
            var newActorId = $"{playerName}-actor";
            var playerActor = ActorProxy.Create<IPlayerActor>(new ActorId(newActorId), new Uri(PlayerActorUri));
            await playerActor.InitializeAsync(Id.ToString(), playerName, cancellationToken);

            await StateManager.AddOrUpdateStateAsync("players", new List<string>() { newActorId }, (key, value) =>
               {
                   if (!value.Contains(newActorId))
                   {
                       value.Add(newActorId);
                   }

                   return value;
               }, cancellationToken);

            var ev = GetEvent<IGameEvents>();
            ev.NewPlayerJoined(Id.ToString(), playerName);

            return newActorId;
        }

        public async Task<IList<PlayerInfo>> GetLatestPlayerInfoAsync(CancellationToken cancellationToken)
        {
            var allPlayers = await StateManager.GetStateAsync<List<string>>("players", cancellationToken);

            var tasks = allPlayers.Select(actorName =>
            {
                var playerActor = ActorProxy.Create<IPlayerActor>(new ActorId(actorName), new Uri(PlayerActorUri));
                return playerActor.GetLatestInfoAsync(cancellationToken);
            }).ToList();

            await Task.WhenAll(tasks);

            return tasks
                .Select(t => t.Result)
                .ToList();
        }

        public async Task NotifyPlayerMovedAsync(PlayerInfo lastMovement, CancellationToken cancellationToken)
        {
            var ev = GetEvent<IGameEvents>();
            ev.ScoreboardUpdated(lastMovement);
        }
    }
}
