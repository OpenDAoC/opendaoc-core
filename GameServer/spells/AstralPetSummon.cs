
using System;

using System.Collections.Generic;
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.Language;
using DOL.Database;
using DOL.GS.Styles;

namespace DOL.GS.Spells
{
    
   [SpellHandler("AstralPetSummon")]
    public class AstralPetSummon : SummonSpellHandler
    {
    	//Graveen: Not implemented property - can be interesting
        /* 
        public bool Controllable
        {
            get { return false; }
        }*/

        /// <summary>
        /// Summon the pet.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="effectiveness"></param>
        public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
        {
            base.ApplyEffectOnTarget(target, effectiveness);

            m_pet.TempProperties.setProperty("target", target);
            (m_pet.Brain as IOldAggressiveBrain).AddToAggroList(target, 1);
            (m_pet.Brain as ProcPetBrain).Think();

        }

        protected override GameSummonedPet GetGamePet(INpcTemplate template) { return new AstralPet(template); }
        protected override IControlledBrain GetPetBrain(GameLiving owner) { return new ProcPetBrain(owner); }
        protected override void SetBrainToOwner(IControlledBrain brain) {}

        protected override void OnNpcReleaseCommand(CoreEvent e, object sender, EventArgs arguments)
        {
            if (!(sender is GameNPC) || !((sender as GameNPC).Brain is IControlledBrain))
                return;
            GameNPC pet = sender as GameNPC;
            IControlledBrain brain = pet.Brain as IControlledBrain;

            GameEventMgr.RemoveHandler(pet, GameLivingEvent.PetReleased, new CoreEventHandler(OnNpcReleaseCommand));

            DOL.GS.Effects.GameSpellEffect effect = FindEffectOnTarget(pet, this);
            if (effect != null)
                effect.Cancel(false);
        }

        protected override void GetPetLocation(out int x, out int y, out int z, out ushort heading, out Region region)
        {
            base.GetPetLocation(out x, out y, out z, out heading, out region);
            heading = Caster.Heading;
        }

         public AstralPetSummon(GameLiving caster, Spell spell, SpellLine line)
            : base(caster, spell, line) { }
    }
}

namespace DOL.GS
{
    public class AstralPet : GameSummonedPet
    {
        public override int MaxHealth
        {
            get { return Level * 10; }
        }

        public override void OnAttackedByEnemy(AttackData ad) { }
        public AstralPet(INpcTemplate npcTemplate) : base(npcTemplate) { }
    }
}