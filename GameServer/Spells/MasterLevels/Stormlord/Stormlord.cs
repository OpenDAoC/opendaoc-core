using Core.Database;
using Core.Database.Tables;
using Core.GS.Effects;
using Core.GS.Enums;
using Core.GS.Skills;

namespace Core.GS.Spells
{
    
    [SpellHandler("StormMissHit")]
    public class StormMissHit : MasterLevelBuffHandling
    {
        public override EProperty Property1 { get { return EProperty.MissHit; } }

        public override void ApplyEffectOnTarget(GameLiving target)
        {
            GameSpellEffect neweffect = CreateSpellEffect(target, Effectiveness);
            if (target == null) return;
            if (!target.IsAlive || target.ObjectState != GameLiving.eObjectState.Active) return;
            neweffect.Start(target);

            if (target is GamePlayer)
                ((GamePlayer)target).Out.SendMessage("You're harder to hit!", EChatType.CT_YouWereHit, EChatLoc.CL_SystemWindow);

        }

        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {

            effect.Owner.EffectList.Remove(effect);
            return base.OnEffectExpires(effect, noMessages);
        }

        public override int CalculateSpellResistChance(GameLiving target)
        {
            return 0;
        }

        // constructor
        public StormMissHit(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }

    //shared timer 1
    #region Stormlord-7
    [SpellHandler("ChokingVapors")]
    public class ChokingVaporsSpell : StormSpellHandler
    {
        // constructor
        public ChokingVaporsSpell(GameLiving caster, Spell spell, SpellLine line)
            : base(caster, spell, line)
        {
            //Construct a new storm.
            storm = new GameStorm();
            storm.Realm = caster.Realm;
            storm.X = caster.X;
            storm.Y = caster.Y;
            storm.Z = caster.Z;
            storm.CurrentRegionID = caster.CurrentRegionID;
            storm.Heading = caster.Heading;
            storm.Owner = (GamePlayer)caster;
            storm.Movable = true;

            // Construct the storm spell
            dbs = new DbSpell();
            dbs.Name = spell.Name;
            dbs.Icon = 7223;
            dbs.ClientEffect = 7223;
            dbs.Damage = spell.Damage;
            dbs.DamageType = (int)spell.DamageType;
            dbs.Target = "Enemy";
            dbs.Radius = 0;
            dbs.Type = ESpellType.StormStrConstDebuff.ToString();
            dbs.Value = spell.Value;
            dbs.Duration = spell.ResurrectHealth; // should be 2
            dbs.Frequency = spell.ResurrectMana;
            dbs.Pulse = 0;
            dbs.PulsePower = 0;
            dbs.LifeDrainReturn = spell.LifeDrainReturn;
            dbs.Power = 0;
            dbs.CastTime = 0;
            dbs.Range = WorldMgr.VISIBILITY_DISTANCE;
            sRadius = 350;
            s = new Spell(dbs, 1);
            sl = SkillBase.GetSpellLine(GlobalSpellsLines.Reserved_Spells);
            tempest = ScriptMgr.CreateSpellHandler(m_caster, s, sl);
        }
    }
    /// <summary>
    /// Str/Con stat specline debuff
    /// </summary>
    [SpellHandler("StormStrConstDebuff")]
    public class StormStrConstDebuff : DualStatDebuff
    {
        public override EProperty Property1 { get { return EProperty.Strength; } }
        public override EProperty Property2 { get { return EProperty.Constitution; } }

        public override void ApplyEffectOnTarget(GameLiving target)
        {
            GameSpellEffect neweffect = CreateSpellEffect(target, Effectiveness);
            if (target == null) return;
            if (!target.IsAlive || target.ObjectState != GameLiving.eObjectState.Active) return;
            neweffect.Start(target);

            if (target is GamePlayer)
                ((GamePlayer)target).Out.SendMessage("Your strenght and constitution decreased!", EChatType.CT_YouWereHit, EChatLoc.CL_SystemWindow);

        }

        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {

            effect.Owner.EffectList.Remove(effect);
            return base.OnEffectExpires(effect, noMessages);
        }

        public override int CalculateSpellResistChance(GameLiving target)
        {
            return 0;
        }

        // constructor
        public StormStrConstDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    #endregion

    //shared timer 1
    #region Stormlord-8
    [SpellHandler("SenseDullingCloud")]
    public class BlindingCloudSpell : StormSpellHandler
    {
        // constructor
        public BlindingCloudSpell(GameLiving caster, Spell spell, SpellLine line)
            : base(caster, spell, line)
        {
            //Construct a new storm.
            storm = new GameStorm();
            storm.Realm = caster.Realm;
            storm.X = caster.X;
            storm.Y = caster.Y;
            storm.Z = caster.Z;
            storm.CurrentRegionID = caster.CurrentRegionID;
            storm.Heading = caster.Heading;
            storm.Owner = (GamePlayer)caster;
            storm.Movable = true;

            // Construct the storm spell
            dbs = new DbSpell();
            dbs.Name = spell.Name;
            dbs.Icon = 7305;
            dbs.ClientEffect = 7305;
            dbs.Damage = spell.Damage;
            dbs.DamageType = (int)spell.DamageType;
            dbs.Target = "Enemy";
            dbs.Radius = 0;
            dbs.Type = ESpellType.StormAcuityDebuff.ToString();
            dbs.Value = spell.Value;
            dbs.Duration = spell.ResurrectHealth; // should be 2
            dbs.Frequency = spell.ResurrectMana;
            dbs.Pulse = 0;
            dbs.PulsePower = 0;
            dbs.LifeDrainReturn = spell.LifeDrainReturn;
            dbs.Power = 0;
            dbs.CastTime = 0;
            dbs.Range = WorldMgr.VISIBILITY_DISTANCE;
            sRadius = 350;
            s = new Spell(dbs, 1);
            sl = SkillBase.GetSpellLine(GlobalSpellsLines.Reserved_Spells);
            tempest = ScriptMgr.CreateSpellHandler(m_caster, s, sl);
        }
    }
    /// <summary>
    /// Acuity stat baseline debuff
    /// </summary>
    [SpellHandler("StormAcuityDebuff")]
    public class StormAcuityDebuff : SingleStatDebuff
    {
        public override EProperty Property1
        {

            get
            {
                EProperty temp = EProperty.Acuity;
                if (Target.Realm == ERealm.Albion) temp = EProperty.Intelligence;
                if (Target.Realm == ERealm.Midgard) temp = EProperty.Piety;
                if (Target.Realm == ERealm.Hibernia) temp = EProperty.Intelligence;

                return temp;
            }
        }

        public override void ApplyEffectOnTarget(GameLiving target)
        {
            GameSpellEffect neweffect = CreateSpellEffect(target, Effectiveness);
            if (target == null) return;
            if (!target.IsAlive || target.ObjectState != GameLiving.eObjectState.Active) return;
            neweffect.Start(target);

            if (target is GamePlayer)
                ((GamePlayer)target).Out.SendMessage("Your acuity decreased!", EChatType.CT_YouWereHit, EChatLoc.CL_SystemWindow);

        }

        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {

            effect.Owner.EffectList.Remove(effect);
            return base.OnEffectExpires(effect, noMessages);
        }

        public override int CalculateSpellResistChance(GameLiving target)
        {
            return 0;
        }

        // constructor
        public StormAcuityDebuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    #endregion
}
