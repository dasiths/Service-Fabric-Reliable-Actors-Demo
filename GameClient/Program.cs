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
            var gameActor = ActorProxy.Create<IGameActor>(new ActorId(gameName), new Uri(GameActorUri));
            await gameActor.SubscribeAsync<IGameEvents>(new GameEventsHandler(gameActor));

            var playerActorId = await gameActor.JoinGameAsync(playerName, CancellationToken.None);
            var playerActor = ActorProxy.Create<IPlayerActor>(new ActorId(playerActorId), new Uri(PlayerActorUri));

            while (true)
            {
                Console.WriteLine("Press return to move to new location...");
                Console.ReadLine();
                
                await playerActor.MoveToAsync(rand.Next(100), rand.Next(100), CancellationToken.None);
            }
        }
    }
}
