using System;
using System.Collections.Generic;
using System.Text;
using GameActor.Interfaces;

namespace GameClient
{
    public class GameEventsHandler: IGameEvents
    {
        public void NewPlayerJoined(string gameName, string newPlayerName)
        {
            Console.WriteLine($"New player '{newPlayerName}' joined the game...");
        }
    }
}
