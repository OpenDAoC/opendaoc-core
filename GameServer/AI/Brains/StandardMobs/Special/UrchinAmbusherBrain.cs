using DOL.GS;

namespace DOL.AI.Brain
{
    public class UrchinAmbusherBrain : StandardMobBrain
    {
        public override void Think()
        {
            base.Think();
        }

        public override void OnAttackedByEnemy(AttackData ad)
        {
            NpcUrchinAmbusher urchinAmbusher = Body as NpcUrchinAmbusher;
            urchinAmbusher.LeaveStealth();
            base.OnAttackedByEnemy(ad);
        }
    }
}
