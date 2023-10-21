using Core.GS.Enums;

namespace Core.GS.Commands;

[Command(
	"&hascredit",
	EPrivLevel.GM,
	"GMCommands.HasCredit.Description",
	"GMCommands.HasCredit.Usage")]
public class HasCreditCommand : ACommandHandler, ICommandHandler
{
	public void OnCommand(GameClient client, string[] args)
	{
		if (client == null || client.Player == null)
		{
			return;
		}

		if (IsSpammingCommand(client.Player, "HasCredit"))
		{
			return;
		}

		// extra check to disallow all but server GM's
		if (client.Account.PrivLevel < 2)
			return;

		if (args.Length < 2)
		{
			DisplaySyntax(client);
			return;
		}

		var target = client.Player.TargetObject as GamePlayer;

		if (target == null)
		{
			DisplayMessage(client.Player,"You need to select a player.");
			return;
		}

		var mob = args[1];

		var hascredit = AchievementUtil.CheckPlayerCredit(mob, target,
			(int) client.Player.Realm);
		
		DisplayMessage(client.Player,$"{target.Name} credit for {mob}: {hascredit}");
	}
}