/*
 *
 * ATLAS ROG command handler
 *
 */
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
    [Command(
        "&eventrog",
        EPrivLevel.Player,
        "Toggle receiving ROGs during the event",
        "/eventrog <on/off>")]
    public class EventROGCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (args.Length < 2)
            {
                switch (client.Player.ReceiveROG)
                {
                    case true:
                        client.Out.SendMessage("Your Event ROG flag is ON.", EChatType.CT_Important, EChatLoc.CL_SystemWindow);
                        break;
                    case false:
                        client.Out.SendMessage("Your Event ROG flag is OFF.", EChatType.CT_Important, EChatLoc.CL_SystemWindow);
                        break;
                }
                DisplaySyntax(client);
                return;
            }

            if (IsSpammingCommand(client.Player, "eventrog"))
                return;

            if (args[1].ToLower().Equals("on"))
            {
                client.Player.ReceiveROG = true;
                client.Player.SaveIntoDatabase();
                client.Out.SendMessage("Your Event ROG flag is ON. You will receive ROGs for your kills.\nUse '/eventrog off' to stop receiving them.", EChatType.CT_Important, EChatLoc.CL_SystemWindow);
            }
            else if (args[1].ToLower().Equals("off"))
            {
                client.Player.ReceiveROG = false;
                client.Player.SaveIntoDatabase();
                client.Out.SendMessage("Your Event ROG flag is OFF. You will no longer receive ROGs for your kills.\nUse '/eventrog on' to start receiving them again.", EChatType.CT_Important, EChatLoc.CL_SystemWindow);
            }

        }
    }
}