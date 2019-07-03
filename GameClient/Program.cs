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

        static void Main(string[] args)
        {
            RunDemo("Dasith's game").ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task RunDemo(string gameName)
        {
            Console.WriteLine("Hit return when the service is up...");
            Console.ReadLine();
            Console.WriteLine("Enter your name:");
            var playerName = Console.ReadLine();

            Console.WriteLine("This might take a few seconds...");
            var actor = ActorProxy.Create<IGameActor>(new ActorId(gameName), new Uri(GameActorUri));
            await actor.SubscribeAsync<IGameEvents>(new GameEventsHandler());
            await actor.JoinGameAsync(playerName);

            while (true)
            {
                var value = await actor.GetCountAsync(new CancellationToken());
                Console.WriteLine(value);
                await actor.SetCountAsync(value + 1, new CancellationToken());
                Console.ReadLine();
            }
        }
    }
}
