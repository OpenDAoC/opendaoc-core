/*
 * [Ganrod] Nidel 2008-07-08
 * - Useless using removed
 * - Get Main Pet tank or Main Pet caster by spell damage type
 */
using DOL.AI.Brain;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Spells
{
  /// <summary>
  /// Spell handler to summon a animist pet.
  /// </summary>
  /// <author>IST</author>
  [SpellHandler("SummonAnimistPet")]
  public class SummonAnimistMainPet : SummonAnimistPet
  {
    public SummonAnimistMainPet(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
    {
    }

    public override bool CheckEndCast(GameLiving selectedTarget)
    {
      if(Caster is GamePlayer && Caster.ControlledBrain != null)
      {
        MessageToCaster(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "SummonAnimistPet.CheckBeginCast.AlreadyHaveaPet"), EChatType.CT_SpellResisted);
        return false;
      }
      return base.CheckEndCast(selectedTarget);
    }

    protected override IControlledBrain GetPetBrain(GameLiving owner)
    {
      if(Spell.DamageType == 0)
      {
        return new TurretMainPetCasterBrain(owner);
      }
      //[Ganrod] Nidel: Spell.DamageType : 1 for tank pet
      if(Spell.DamageType == (EDamageType) 1)
      {
        return new TurretMainPetTankBrain(owner);
      }
      return base.GetPetBrain(owner);
    }
  }
}