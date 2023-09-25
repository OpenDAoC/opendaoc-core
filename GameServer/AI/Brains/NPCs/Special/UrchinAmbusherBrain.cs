using DOL.GS;

namespace DOL.AI.Brain
{
    public class UrchinAmbusherBrain : StandardNpcBrain
    {
        public override void Think()
        {
            base.Think();
        }

        public override void OnAttackedByEnemy(AttackData ad)
        {
            UrchinAmbusher urchinAmbusher = Body as UrchinAmbusher;
            urchinAmbusher.LeaveStealth();
            base.OnAttackedByEnemy(ad);
        }
    }
}
