using System;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

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

        // Trade System
        public static TradePlayer[] TradeActive = new TradePlayer[Main.maxPlayers];

        public TradeRequest(Main game)
            : base(game)
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

        private void OnPlayerCommand(PlayerCommandEventArgs args)
        {
            if (args.Handled || args.Player == null)
                return;

            Command command = args.CommandList.FirstOrDefault();
            if (command == null || (command.Permissions.Any() && !command.Permissions.Any(s => args.Player.Group.HasPermission(s))))
                return;
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
                HelpText = "Get the world data info"
            });
            #endregion
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                PlayerHooks.PlayerCommand -= OnPlayerCommand;
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
