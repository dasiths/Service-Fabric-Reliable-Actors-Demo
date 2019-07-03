using System;
using System.Collections.Generic;
using System.Text;

namespace GameActor.Interfaces.Models
{
    public class PlayerInfo
    {
        public string PlayerName { get; set; }
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }
}
