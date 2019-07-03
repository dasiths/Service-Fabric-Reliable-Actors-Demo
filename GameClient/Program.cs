using System;
using System.Threading;
using GameActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace GameClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameName = "dasith's game";
            Console.WriteLine("Enter your name:");
            var playerName = Console.ReadLine();

            IGameActor actor = ActorProxy.Create<IGameActor>(new ActorId(gameName), new Uri("fabric:/ServiceFabfricReliableActorsDemo/GameActorService"));
            actor.SubscribeAsync<IGameEvents>(new GameEventsHandler());
            actor.JoinGameAsync(playerName);

            while (true)
            {
                var retval = actor.GetCountAsync(new CancellationToken()).GetAwaiter().GetResult();
                Console.WriteLine(retval);
                actor.SetCountAsync(retval + 1, new CancellationToken()).GetAwaiter().GetResult();
                Console.ReadLine();
            }
        }
    }
}
