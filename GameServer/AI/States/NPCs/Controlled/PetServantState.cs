using DOL.GS;

namespace DOL.AI.Brain
{
    public class PetServantState_WakingUp : NpcControlledState_WakingUp
    {
        public PetServantState_WakingUp(PetServantBrain brain) : base(brain)
        {
            StateType = eFSMStateType.WAKING_UP;
        }

        public override void Think()
        {
            base.Think();
        }
    }

    public class PetServantState_Defensive : NpcControlledState_Defensive
    {
        public PetServantState_Defensive(PetServantBrain brain) : base(brain)
        {
            StateType = eFSMStateType.IDLE;
        }

        public override void Think()
        {
            PetServantBrain brain = (PetServantBrain) _brain;

            // If spells are queued then handle them first.
            if (brain.HasSpellsQueued())
                brain.CheckSpellQueue();

            base.Think();
        }
    }

    public class PetServantState_Aggro : NpcControlledState_Aggro
    {
        public PetServantState_Aggro(PetServantBrain brain) : base(brain)
        {
            StateType = eFSMStateType.AGGRO;
        }

        public override void Think()
        {
            PetServantBrain brain = (PetServantBrain) _brain;

            // If spells are queued then handle them first.
            if (brain.HasSpellsQueued())
                brain.CheckSpellQueue();

            base.Think();
        }
    }

    public class PetServantState_Passive : NpcControlledState_Passive
    {
        public PetServantState_Passive(PetServantBrain brain) : base(brain)
        {
            StateType = eFSMStateType.PASSIVE;
        }

        public override void Think()
        {
            PetServantBrain brain = (PetServantBrain) _brain;

            // If spells are queued then handle them first.
            if (brain.HasSpellsQueued())
                brain.CheckSpellQueue();

            base.Think();
        }
    }
}
