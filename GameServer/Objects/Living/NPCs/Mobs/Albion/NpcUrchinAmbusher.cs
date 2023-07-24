using DOL.AI.Brain;

namespace DOL.GS
{
    public class NpcUrchinAmbusher : StealtherMob
    {
        public NpcUrchinAmbusher() : base()
        {
            SetOwnBrain(new UrchinAmbusherBrain());
        }
        public void LeaveStealth()
        {
            Flags &= eFlags.STEALTH;
        }
    }
}
