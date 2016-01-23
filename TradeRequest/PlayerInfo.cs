using TShockAPI;
using System;

namespace TradeRequest
{
    public class TradePlayer
    {
        public bool active = false;
        public int target = 0;
        public int item = 0;
        public DateTime time = DateTime.UtcNow;
        public DateTime confirmlock = DateTime.UtcNow;
    }
}
