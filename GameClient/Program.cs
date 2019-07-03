using System;
using System.Threading;
using System.Threading.Tasks;
using GameActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace GameClient
{
    class Program
    {
        private const string GameActorUri = "fabric:/ReliableActorsDemo/GameActorService";
        private const string PlayerActorUri = "fabric:/ReliableActorsDemo/PlayerActorService";

        static void Main(string[] args)
        {
            RunDemo("Dasith's game").ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task RunDemo(string gameName)
        {
            var rand = new Random();
            Console.WriteLine("Hit return when the service is up...");
            Console.ReadLine();
            Console.WriteLine("Enter your name:");
            var playerName = Console.ReadLine();

            Console.WriteLine("This might take a few seconds...");
            var actor = ActorProxy.Create<IGameActor>(new ActorId(gameName), new Uri(GameActorUri));
            await actor.SubscribeAsync<IGameEvents>(new GameEventsHandler());
            var playerActorId = await actor.JoinGameAsync(playerName, CancellationToken.None);

            while (true)
            {
                var value = await actor.GetCountAsync(new CancellationToken());
                Console.WriteLine(value);
                await actor.SetCountAsync(value + 1, new CancellationToken());

                var playerActor = ActorProxy.Create<IPlayerActor>(new ActorId(playerActorId), new Uri(PlayerActorUri));
                await playerActor.MoveToAsync(rand.Next(100), rand.Next(100), CancellationToken.None);

                var positions = await actor.GetLatestPlayerInfoAsync(CancellationToken.None);

                foreach (var playerInfo in positions)
                {
                    Console.WriteLine($"Position of {playerInfo.PlayerName} is ({playerInfo.XCoordinate},{playerInfo.YCoordinate})." +
                                      $"\nUpdated at {playerInfo.LastUpdate}\n");
                }

                Console.ReadLine();
            }
        }
    }
}
