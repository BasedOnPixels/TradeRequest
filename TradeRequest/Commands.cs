﻿using System;
using System.Linq;
using Terraria;
using TShockAPI;

namespace TradeRequest
{
    class Commands
    {
        public static void Trade(CommandArgs args)
        {
            if (!Main.ServerSideCharacter)
            {
                args.Player.SendErrorMessage("TShock Server Sided Characters is disabled.");
                return;
            }
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /trade <player/id>");
                return;
            }

            var player = args.Player;
            if (player.TPlayer.trashItem.netID == 0)
            {
                player.SendErrorMessage("You must have an item in the Trash Bin to trade.");
                return;
            }

            if (args.Parameters[0] == "confirm")
            {
                TradePlayer trader = TradeRequest.TradeActive[player.Index];
                TradePlayer target = TradeRequest.TradeActive[trader.target];
                if ((DateTime.UtcNow - trader.confirmlock).TotalSeconds > 3)
                {
                    #region Trade Section
                    TSPlayer targetplayer = TShock.Players[trader.target];
                    if (trader.active && target.active)
                    {
                        trader.active = false;
                        target.active = false;
                        var pItem = trader.item;
                        var pStack = player.TPlayer.trashItem.stack;
                        var pPrefix = player.TPlayer.trashItem.prefix;
                        var tItem = target.item;
                        var tStack = targetplayer.TPlayer.trashItem.stack;
                        var tPrefix = targetplayer.TPlayer.trashItem.prefix;

                        player.TPlayer.trashItem.netDefaults(tItem);
                        if (player.TPlayer.trashItem.netID != 0)
                        {
                            player.TPlayer.trashItem.stack = tStack;
                            player.TPlayer.trashItem.prefix = tPrefix;
                        }

                        targetplayer.TPlayer.trashItem.netDefaults(pItem);
                        if (targetplayer.TPlayer.trashItem.netID != 0)
                        {
                            targetplayer.TPlayer.trashItem.stack = pStack;
                            targetplayer.TPlayer.trashItem.prefix = pPrefix;
                        }

                        NetMessage.SendData(5, -1, -1, Main.player[player.Index].trashItem.name, player.Index, 179, (float)tPrefix);
                        NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].trashItem.name, player.Index, 179, (float)tPrefix);
                        NetMessage.SendData(5, -1, -1, Main.player[targetplayer.Index].trashItem.name, targetplayer.Index, 179, (float)pPrefix);
                        NetMessage.SendData(5, targetplayer.Index, -1, Main.player[targetplayer.Index].trashItem.name, targetplayer.Index, 179, (float)pPrefix);
                        TSPlayer.All.SendInfoMessage("{0} ([i/s{1}:{2}]) traded successfully with {3} ([i/s{4}:{5}]).", player.Name, pStack, pItem, targetplayer.Name, tStack, tItem);
                        TShock.Log.ConsoleInfo("TRADEINFO: {0} traded x{1} {2} for {3}'s x{4} {5}", player.Name, pStack, Main.player[targetplayer.Index].trashItem.name, targetplayer.Name, tStack, Main.player[player.Index].trashItem.name);
                        return;
                    }
                    else if ((trader.active && trader.item != player.TPlayer.trashItem.netID) || (target.active && target.item != targetplayer.TPlayer.trashItem.netID))
                    {
                        player.SendErrorMessage("Trade Failed. Item is no longer in the Trash Bin or Scamming.");
                        return;
                    }
                    else
                    {
                        player.SendInfoMessage("You must send a trade request. Please use /trade <player>");
                        return;
                    }
                    #endregion
                }
                else
                    args.Player.SendErrorMessage("You must wait a few seconds before finalizing the trade.");
            }

            var foundplr = TShock.Utils.FindPlayer(args.Parameters[0]);
            if (foundplr.Count == 0)
            {
                args.Player.SendErrorMessage("Invalid player!");
                return;
            }
            else if (foundplr.Count > 1)
            {
                TShock.Utils.SendMultipleMatchError(args.Player, foundplr.Select(p => p.Name));
                return;
            }
            else
            {
                var found = foundplr[0];
                if (found.Index == args.Player.Index)
                {
                    args.Player.SendErrorMessage("You cannot trade with yourself.");
                    return;
                }

                TradePlayer trader = TradeRequest.TradeActive[player.Index];
                if (!trader.active)
                {
                    trader.active = true;
                    trader.target = found.Index;
                    trader.item = player.TPlayer.trashItem.netID;
                    trader.time = DateTime.UtcNow;
                    TradePlayer target = TradeRequest.TradeActive[found.Index];
                    if (target.active == true)
                    {
                        target.time = DateTime.UtcNow;
                        trader.confirmlock = DateTime.UtcNow;
                        target.confirmlock = DateTime.UtcNow;
                    }
                }
                else
                {
                    player.SendErrorMessage("You already have a trade active.");
                    return;
                }
                player.SendInfoMessage("You have sent a trade request to {0}. Cancel the trade by removing the item.", found.Name);
                found.SendInfoMessage("{0} would like to trade [i/s{1}:{2}]. Type [c/FFFFFF:/trade confirm] after [c/FFFFFF:/trade {0}].", player.Name, player.TPlayer.trashItem.stack, player.TPlayer.trashItem.netID);
            }
        }
    }
}
