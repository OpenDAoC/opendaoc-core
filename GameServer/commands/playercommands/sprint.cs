using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&sprint",
		ePrivLevel.Player,
		"Toggles sprint mode",
		"/sprint")]
	public class SprintCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (client.Player.HasAbility(Abilities.Sprint))
			{
				client.Player.Sprint(!client.Player.IsSprinting);
			}
			else
			{
				client.Out.SendMessage("You do not have a sprint ability.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
			}
		}
	}
}