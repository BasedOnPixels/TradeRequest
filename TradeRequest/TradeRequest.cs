﻿using System;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TradeRequest
{
    [ApiVersion(1, 22)]
    public class TradeRequest : TerrariaPlugin
    {
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Name
        {
            get { return "Trade Request"; }
        }
        public override string Author
        {
            get { return "Marcus101RR, NicatronTG, Enerdy"; }
        }
        public override string Description
        {
            get { return "Allow trading with other players."; }
        }

        public static TradePlayer[] TradeActive = new TradePlayer[Main.maxPlayers];

        public TradeRequest(Main game) : base(game)
        {
            Order = 9;

            for (int i = 0; i < Main.maxPlayers; i++)
                TradeActive[i] = new TradePlayer();
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            GetDataHandlers.InitGetDataHandler();
        }

        private void OnInitialize(EventArgs args)
        {
            #region Commands
            Action<Command> Add = c =>
            {
                TShockAPI.Commands.ChatCommands.RemoveAll(c2 => c2.Names.Select(s => s.ToLowerInvariant()).Intersect(c.Names.Select(s => s.ToLowerInvariant())).Any());
                TShockAPI.Commands.ChatCommands.Add(c);
            };

            Add(new Command(Permissions.canchat, Commands.Trade, "trade")
            {
                AllowServer = true,
                HelpText = "Allows the player to trade with another player."
            });
            #endregion
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }

        private void OnUpdate(EventArgs args)
        {
            #region Vanilla Trading Timer
            for (int i = 0; i < Main.maxNetPlayers; i++)
            {
                TSPlayer trader = TShock.Players[i];
                if (trader != null && trader.Active)
                {
                    TradePlayer iTrade = TradeActive[i];
                    TSPlayer target = TShock.Players[iTrade.target];

                    if (iTrade.active && (DateTime.UtcNow - iTrade.time).TotalSeconds >= 30)
                    {
                        iTrade.active = false;
                        trader.SendErrorMessage("Your trade request has ended.");
                        target.SendErrorMessage("{0} trade request has ended.", target.Name);
                    }
                    if (iTrade.active && iTrade.item != trader.TPlayer.trashItem.netID)
                    {
                        iTrade.active = false;
                        TSPlayer.All.SendErrorMessage("{0} has canceled the trade.", trader.Name);
                    }
                }
            }
            #endregion
        }
    }
}
