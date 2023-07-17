using DOL.Database;
using DOL.GS.PacketHandler;
using System.Collections;

namespace DOL.GS.Commands
{
	[Command(
		"&vault",
		EPrivLevel.Player,
		"Open the player's inventory.")]
	public class VaultCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if ((ServerProperties.Properties.ALLOW_VAULT_COMMAND || client.Account.PrivLevel > 1)
				&& client.Player is GamePlayer player && player.Inventory is IGameInventory inventory)
					player.Out.SendInventoryItemsUpdate(eInventoryWindowType.PlayerVault, inventory.AllItems);
		}
	}
}