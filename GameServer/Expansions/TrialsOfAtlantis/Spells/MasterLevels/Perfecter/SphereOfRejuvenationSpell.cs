using Core.Database.Tables;
using Core.GS.Enums;
using Core.GS.Scripts;
using Core.GS.Skills;
using Core.GS.Spells;
using Core.GS.World;

namespace Core.GS.Expansions.TrialsOfAtlantis.Spells.MasterLevels;

//shared timer 1

[SpellHandler("FOH")]
public class SphereOfRejuvenationSpell : FontSpellHandler
{
    // constructor
    public SphereOfRejuvenationSpell(GameLiving caster, Spell spell, SpellLine line)
        : base(caster, spell, line)
    {
        ApplyOnNPC = true;

        //Construct a new font.
        font = new GameFont();
        font.Model = 2585;
        font.Name = spell.Name;
        font.Realm = caster.Realm;
        font.X = caster.X;
        font.Y = caster.Y;
        font.Z = caster.Z;
        font.CurrentRegionID = caster.CurrentRegionID;
        font.Heading = caster.Heading;
        font.Owner = (GamePlayer)caster;

        // Construct the font spell
        dbs = new DbSpell();
        dbs.Name = spell.Name;
        dbs.Icon = 7245;
        dbs.ClientEffect = 7245;
        dbs.Damage = spell.Damage;
        dbs.DamageType = (int)spell.DamageType;
        dbs.Target = "Realm";
        dbs.Radius = 0;
        dbs.Type = ESpellType.HealOverTime.ToString();
        dbs.Value = spell.Value;
        dbs.Duration = spell.ResurrectHealth;
        dbs.Frequency = spell.ResurrectMana;
        dbs.Pulse = 0;
        dbs.PulsePower = 0;
        dbs.LifeDrainReturn = spell.LifeDrainReturn;
        dbs.Power = 0;
        dbs.CastTime = 0;
        dbs.Range = WorldMgr.VISIBILITY_DISTANCE;
        dbs.Message1 = spell.Message1;
        dbs.Message2 = spell.Message2;
        dbs.Message3 = spell.Message3;
        dbs.Message4 = spell.Message4;
        sRadius = 350;
        s = new Spell(dbs, 1);
        sl = SkillBase.GetSpellLine(GlobalSpellsLines.Reserved_Spells);
        heal = ScriptMgr.CreateSpellHandler(m_caster, s, sl);
    }
}