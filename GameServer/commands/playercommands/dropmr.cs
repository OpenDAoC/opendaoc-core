namespace DOL.GS.Commands
{
    [CmdAttribute("&dropmr", //command to handle
        ePrivLevel.Player, //minimum privelege level
        "Drops the Minotaurrelic.", //command description
        "/dropmr")] //usage
    public class DropmrCommandHandler : ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (client.Player.MinotaurRelic == null) return;

            client.Player.MinotaurRelic.PlayerLoosesRelic(client.Player, false);
        }
    }
}