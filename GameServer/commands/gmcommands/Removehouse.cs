using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	/// <summary>
	/// Command handler to remove House player
	/// </summary>
	[CmdAttribute(
		"&removehouse",
		ePrivLevel.GM,
		"Remove House or you are", "/removehouse")]
	public class RemoveHouseCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		/// <summary>
		/// Method to handle the command from the client
		/// </summary>
		/// <param name="client"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public void OnCommand(GameClient client, string[] args)
		{
			DOL.GS.Housing.HouseMgr.RemoveHouse(client.Player.CurrentHouse);
		}
	}
}