using System;
using DOL.GS.PacketHandler;
using DOL.GS.PacketHandler.Client.v168;

namespace DOL.GS.Commands;

[Command("&password", EPrivLevel.Player,
	"Changes your account password",
	"/password <current_password> <new_password>")]
public class PasswordCommand : ACommandHandler, ICommandHandler
{
	private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	private const string PASSWORD_PROPERTY = "PasswordCommandProperty";

	#region ICommandHandler Members

	public void OnCommand(GameClient client, string[] args)
	{
		string usage = "Usage: /password <current_password> <new_password>";

		if (args.Length < 3)
		{
			client.Out.SendMessage(usage, EChatType.CT_System, EChatLoc.CL_SystemWindow);
			return;
		}
		try
		{
			string oldPassword = args[1];
			string newPassword = args[2];

			if ((client.Account != null) && (LoginRequestHandler.CryptPassword(oldPassword) == client.Account.Password))
			{
				// TODO: Add confirmation dialog
				// TODO: If user has set their email address, mail them the change notification
				client.Player.TempProperties.SetProperty(PASSWORD_PROPERTY, newPassword);
				client.Out.SendCustomDialog("Do you wish to change your password to \n" + newPassword, PasswordCheckCallback);
			}
			else
			{
				client.Out.SendMessage("Your current password was incorrect.", EChatType.CT_Important, EChatLoc.CL_SystemWindow);

				if (log.IsInfoEnabled)
					log.Info(client.Player.Name + " (" + client.Account.Name + ") attempted to change password but failed!");

				return;
			}
		}
		catch (Exception)
		{
			client.Out.SendMessage(usage, EChatType.CT_System, EChatLoc.CL_SystemWindow);
		}
	}

	#endregion

	private void PasswordCheckCallback(GamePlayer player, byte response)
	{
		if (response != 0x01)
			return;

		var newPassword = player.TempProperties.GetProperty<string>(PASSWORD_PROPERTY, null);
		if (newPassword == null)
			return;

		player.TempProperties.RemoveProperty(PASSWORD_PROPERTY);
		player.Out.SendMessage("Your password has been changed.", EChatType.CT_Important, EChatLoc.CL_SystemWindow);
		player.Client.Account.Password = LoginRequestHandler.CryptPassword(newPassword);

		GameServer.Database.SaveObject(player.Client.Account);

		// Log change
		AuditMgr.AddAuditEntry(player, EAuditType.Account, EAuditSubType.AccountPasswordChange, "", player.Name);

		if (log.IsInfoEnabled)
			log.Info(player.Name + " (" + player.Client.Account.Name + ") changed password.");
	}
}