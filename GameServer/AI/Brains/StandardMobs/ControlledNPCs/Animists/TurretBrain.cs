using System;
using System.Collections.Generic;
using System.Linq;
using DOL.GS;

namespace DOL.AI.Brain
{
    public class TurretBrain : ControlledNpcBrain
    {
        protected readonly List<GameLiving> m_defensiveSpellTargets;

        public TurretBrain(GameLiving owner) : base(owner)
        {
            m_defensiveSpellTargets = new();
        }

        public List<GameLiving> DefensiveSpellTargets => m_defensiveSpellTargets;
        public override int AggroRange => ((TurretPet) Body).TurretSpell.Range;

        public override void Think()
        {
            GamePlayer playerowner = GetPlayerOwner();

            if (!playerowner.Client.GameObjectUpdateArray.TryGetValue(new Tuple<ushort, ushort>(Body.CurrentRegionID, (ushort)Body.ObjectID), out long lastUpdate))
                playerowner.Client.GameObjectUpdateArray.TryAdd(new Tuple<ushort, ushort>(Body.CurrentRegionID, (ushort)Body.ObjectID), lastUpdate);

            if (playerowner != null && (GameLoop.GameLoopTime - playerowner.Client.GameObjectUpdateArray[new Tuple<ushort, ushort>(Body.CurrentRegionID, (ushort)Body.ObjectID)]) > ThinkInterval)
                playerowner.Out.SendObjectUpdate(Body);

            if (AggressionState == EAggressionState.Aggressive)
                CheckProximityAggro();

            if (!CheckSpells(eCheckSpellType.Defensive))
                CheckSpells(eCheckSpellType.Offensive);
        }

        public override bool CheckSpells(eCheckSpellType type)
        {
            if (Body == null || AggressionState == EAggressionState.Passive || ((TurretPet)Body).TurretSpell == null)
                return false;

            Spell spell = ((TurretPet)Body).TurretSpell;

            if (Body.GetSkillDisabledDuration(spell) != 0)
                return false;

            bool casted = false;

            switch (type)
            {
                case eCheckSpellType.Defensive:
                    casted = CheckDefensiveSpells(spell);
                    break;
                case eCheckSpellType.Offensive:
                    casted = CheckOffensiveSpells(spell);
                    break;
            }

            return casted /*|| Body.IsCasting*/;
        }

        protected override bool CheckDefensiveSpells(Spell spell)
        {
            switch (spell.SpellType)
            {
                case ESpellType.HeatColdMatterBuff:
                case ESpellType.BodySpiritEnergyBuff:
                case ESpellType.ArmorAbsorptionBuff:
                case ESpellType.AblativeArmor:
                    return TrustCast(spell, eCheckSpellType.Defensive, GetDefensiveTarget(spell));
            }

            return false;
        }

        protected override bool CheckOffensiveSpells(Spell spell)
        {
            switch (spell.SpellType)
            {
                case ESpellType.DirectDamage:
                case ESpellType.DamageSpeedDecrease:
                case ESpellType.SpeedDecrease:
                case ESpellType.Taunt:
                case ESpellType.MeleeDamageDebuff:
                    return TrustCast(spell, eCheckSpellType.Offensive, CalculateNextAttackTarget());
            }

            return false;
        }

        protected virtual bool TrustCast(Spell spell, eCheckSpellType type, GameLiving target)
        {
            if (spell.IsPBAoE)
                return Body.CastSpell(spell, m_mobSpellLine);

            if (target != null)
            {
                Body.TargetObject = target;
                Body.StopAttack();
                return Body.CastSpell(spell, m_mobSpellLine, false);
            }

            return false;
        }

        private GameLiving GetDefensiveTarget(Spell spell)
        {
            // Clear the current list of invalid or already buffed targets before checking nearby players and NPCs.
            for (int i = DefensiveSpellTargets.Count - 1; i >= 0; i--)
            {
                GameLiving living = DefensiveSpellTargets[i];

                if (GameServer.ServerRules.IsAllowedToAttack(Body, living, true) || !living.IsAlive || LivingHasEffect(living, spell) || !Body.IsWithinRadius(living, (ushort)spell.Range))
                    DefensiveSpellTargets.RemoveAt(i);
            }

            foreach (GamePlayer player in Body.GetPlayersInRadius((ushort)spell.Range, !Body.CurrentRegion.IsDungeon))
            {
                if (GameServer.ServerRules.IsAllowedToAttack(Body, player, true) || !player.IsAlive || LivingHasEffect(player, spell))
                    continue;

                if (player == GetPlayerOwner())
                    return player;

                if (!DefensiveSpellTargets.Contains((GameLiving)player))
                    DefensiveSpellTargets.Add(player);
            }

            foreach (GameNPC npc in Body.GetNPCsInRadius((ushort)spell.Range, !Body.CurrentRegion.IsDungeon))
            {
                if (GameServer.ServerRules.IsAllowedToAttack(Body, npc, true) || !npc.IsAlive || LivingHasEffect(npc, spell))
                    continue;

                if (npc == Body || npc == GetLivingOwner())
                    return npc;

                if (!DefensiveSpellTargets.Contains(npc))
                    DefensiveSpellTargets.Add(npc);
            }

            return Enumerable.Any(DefensiveSpellTargets) ? DefensiveSpellTargets[UtilCollection.Random(DefensiveSpellTargets.Count - 1)] : null;
        }

        public override bool Stop()
        {
            ClearAggroList();
            DefensiveSpellTargets.Clear();
            return base.Stop();
        }

        #region AI

        public override void FollowOwner() { }

        public override void Follow(GameObject target) { }

        protected override void OnFollowLostTarget(GameObject target) { }

        public override void Goto(GameObject target) { }

        public override void ComeHere() { }

        public override void Stay() { }

        #endregion
    }
}