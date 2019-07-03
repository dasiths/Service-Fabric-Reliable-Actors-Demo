using Microsoft.ServiceFabric.Actors;

namespace GameActor.Interfaces
{
    public interface IGameEvents : IActorEvents
    {
        void NewPlayerJoined(string gameName, string newPlayerName);
    }
}