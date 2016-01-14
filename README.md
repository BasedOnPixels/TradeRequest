# TradeRequest
Allows players to trade items between one another vai Trash Bin Slot.

TradeRequest allows users to securely trade items between one another without scamming margins. When a player sends a trade request, the recipient has the choice to offering a request back for equal trade, or let it expire. Once both player see what items have been put out for request, either of the players can confirm the finalization of their trade. If an item is of non-interest, or falsely advertised, both users have the opportunity to end the trade by taking the item out of the Trash Bin Slot. Effectively, canceling the trade request.

Features
- Uses Trash Bin Slot as the trade request GUI Slot.
- Multiple users can request trade offers to the same person. The receiver can choose by typing /trade <player>.
- Items removed from Trash Bin during trade are automatically canceled to prevent scamming margin activity.

Commands
- /trade <player/id> - Request a trade to a player.
- /trade confirm - Confirms the trade when both players have offered items.
- Uses permission flag tshock.canchat (May change later)

Actions
During Trade - Remove Item from Trash Bin - Cancels the trade request from the player (preventing scam).
Before Trade - No Item in Trash Bin - Will not allow you to trade, requests you to add an item first.

To-Do
Add SEconomy Support for trading item vs. currency?

Credits
Enerdy - Identifying the mistake of not adding initialization to the Trade Array for all players during plugin startup.
