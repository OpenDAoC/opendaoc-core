namespace DOL.GS.Commands
{
    /// <summary>
    /// Command handler for the /filter command
    /// </summary>
    [Command(
        "&filter",
        ePrivLevel.Player,
        "Turns off the bad word filter.",
        "/filter")]
    public class FilterCommand : AbstractCommandHandler, ICommandHandler
    {
        /// <summary>
        /// Method to handle the command and any arguments
        /// </summary>
        /// <param name="client"></param>
        /// <param name="args"></param>
        public void OnCommand(GameClient client, string[] args)
        {
            // do nothing, just removes the "command doesn't exist" message.
        }
    }
}