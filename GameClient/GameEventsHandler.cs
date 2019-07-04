using System;
using System.Threading;
using System.Threading.Tasks;
using GameActor.Interfaces;
using GameActor.Interfaces.Models;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace GameClient
{
    public class GameEventsHandler : IGameEvents
    {
        private const string GameActorUri = "fabric:/ReliableActorsDemo/GameActorService";
        private readonly IGameActor _gameActor;

        public GameEventsHandler(IGameActor gameActor)
        {
            _gameActor = gameActor; //ActorProxy.Create<IGameActor>(new ActorId(gameName), new Uri(GameActorUri)); ;
        }

        public void NewPlayerJoined(string gameName, string newPlayerName)
        {
            Console.WriteLine($"New player '{newPlayerName}' joined the game...");
        }

        public void ScoreboardUpdated(PlayerInfo lastInfo)
        {
            Console.WriteLine("Scoreboard updated");

            Task.Run(async () =>
                {
                    var positions = await _gameActor.GetLatestPlayerInfoAsync(CancellationToken.None);

                    foreach (var playerInfo in positions)
                    {
                        Console.WriteLine(
                            $"Position of {playerInfo.PlayerName} is ({playerInfo.XCoordinate},{playerInfo.YCoordinate})." +
                            $"\nUpdated at {playerInfo.LastUpdate}\n");
                    }
                }
            );
        }
    }
}
