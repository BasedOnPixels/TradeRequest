using TShockAPI;
using System;

namespace TradeRequest
{
    public class TradePlayer
    {
        public bool active = false;
        public int target;
        public int item;
        public DateTime time = DateTime.UtcNow;
    }
}
