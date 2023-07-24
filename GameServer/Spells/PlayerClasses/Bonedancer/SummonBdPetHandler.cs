using System;
using System.Collections.Generic;
using System.Linq;
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Pet summon spell handler
	///
	/// Spell.LifeDrainReturn is used for pet ID.
	///
	/// Spell.Value is used for hard pet level cap
	/// Spell.Damage is used to set pet level:
	/// less than zero is considered as a percent (0 .. 100+) of target level;
	/// higher than zero is considered as level value.
	/// Resulting value is limited by the Byte field type.
	/// Spell.DamageType is used to determine which type of pet is being cast:
	/// 0 = melee
	/// 1 = healer
	/// 2 = mage
	/// 3 = debuffer
	/// 4 = Buffer
	/// 5 = Range
	/// </summary>
	[SpellHandler("SummonMinion")]
	public class SummonBdPetHandler : SummonHandler
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public SummonBdPetHandler(GameLiving caster, Spell spell, SpellLine line)
			: base(caster, spell, line) { }

		/// <summary>
		/// All checks before any casting begins
		/// </summary>
		/// <param name="selectedTarget"></param>
		/// <returns></returns>
		public override bool CheckBeginCast(GameLiving selectedTarget)
		{
			if (Caster is GamePlayer && ((GamePlayer)Caster).ControlledBrain == null)
			{
                MessageToCaster(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "SummonMinionHandler.CheckBeginCast.Text1"), EChatType.CT_SpellResisted);
                return false;
			}

			if (Caster is GamePlayer && (((GamePlayer)Caster).ControlledBrain.Body.ControlledNpcList == null || ((GamePlayer)Caster).ControlledBrain.Body.PetCount >= ((GamePlayer)Caster).ControlledBrain.Body.ControlledNpcList.Length))
			{
                MessageToCaster(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "SummonMinionHandler.CheckBeginCast.Text2"), EChatType.CT_SpellResisted);

                return false;
			}
			
			if (Caster is GamePlayer && ((GamePlayer) Caster).ControlledBrain != null &&
			    ((GamePlayer) Caster).ControlledBrain.Body.ControlledNpcList != null)
			{
				int cumulativeLevel = 0;
				foreach (var petBrain in ((GamePlayer) Caster).ControlledBrain.Body.ControlledNpcList)
				{
					cumulativeLevel += petBrain != null && petBrain.Body != null ? petBrain.Body.Level : 0;
				}

				byte newpetlevel = (byte)(Caster.Level * m_spell.Damage * -0.01);
				if (newpetlevel > m_spell.Value)
					newpetlevel = (byte)m_spell.Value;

				if (cumulativeLevel + newpetlevel > 75)
				{
					MessageToCaster("Your commander is not powerful enough to control a subpet of this level.", EChatType.CT_SpellResisted);
					return false;
				}
			}
			return base.CheckBeginCast(selectedTarget);
		}

		/// <summary>
		/// Apply effect on target or do spell action if non duration spell
		/// </summary>
		/// <param name="target">target that gets the effect</param>
		/// <param name="effectiveness">factor from 0..1 (0%-100%)</param>
		public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
		{
			if (Caster == null || Caster.ControlledBrain == null)
				return;

			GameNpc temppet = Caster.ControlledBrain.Body;
			//Lets let NPC's able to cast minions.  Here we make sure that the Caster is a GameNPC
			//and that m_controlledNpc is initialized (since we aren't thread safe).
			if (temppet == null)
			{
				if (Caster is GameNpc)
				{
					temppet = (GameNpc)Caster;
					//We'll give default NPCs 2 minions!
					if (temppet.ControlledNpcList == null)
						temppet.InitControlledBrainArray(2);
				}
				else
					return;
			}

			base.ApplyEffectOnTarget(target, effectiveness);

			if (m_pet.Brain is BdPetBrain brain && !brain.MinionsAssisting)
				brain.SetAggressionState(EAggressionState.Passive);

			// Assign weapons
			if (m_pet is BdSubPet subPet)
				switch (subPet.Brain)
				{
					case BdArcherBrain archer:
						subPet.MinionGetWeapon(BdCommanderPet.eWeaponType.OneHandSword);
						subPet.MinionGetWeapon(BdCommanderPet.eWeaponType.Bow);
						break;
					case BbDebufferBrain debuffer:
						subPet.MinionGetWeapon(BdCommanderPet.eWeaponType.OneHandHammer);
						break;
					case BdBufferBrain buffer:
					case BdCasterBrain caster:
						subPet.MinionGetWeapon(BdCommanderPet.eWeaponType.Staff);
						break;
					case BdMeleeBrain melee:
						if(UtilCollection.Chance(60))
							subPet.MinionGetWeapon(BdCommanderPet.eWeaponType.TwoHandAxe);
						else
							subPet.MinionGetWeapon(BdCommanderPet.eWeaponType.OneHandAxe);
						break;
				}
		}

		/// <summary>
		/// Called when owner release NPC
		/// </summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		/// <param name="arguments"></param>
		protected override void OnNpcReleaseCommand(CoreEvent e, object sender, EventArgs arguments)
		{
			GameNpc pet = sender as GameNpc;
			if (pet == null)
				return;

			GameEventMgr.RemoveHandler(pet, GameLivingEvent.PetReleased, new CoreEventHandler(OnNpcReleaseCommand));

			//GameSpellEffect effect = FindEffectOnTarget(pet, this);
			//if (effect != null)
			//	effect.Cancel(false);
			if (pet.effectListComponent.Effects.TryGetValue(EEffect.Pet, out var petEffect))
				EffectService.RequestImmediateCancelEffect(petEffect.FirstOrDefault());
		}

		public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
		{
			if ((effect.Owner is BdPet) && ((effect.Owner as BdPet).Brain is IControlledBrain) && (((effect.Owner as BdPet).Brain as IControlledBrain).Owner is BdCommanderPet))
			{
				BdPet pet = effect.Owner as BdPet;
				BdCommanderPet commander = (pet.Brain as IControlledBrain).Owner as BdCommanderPet;
				commander.RemoveControlledNpc(pet.Brain as IControlledBrain);
			}
			return base.OnEffectExpires(effect, noMessages);
		}

		protected override IControlledBrain GetPetBrain(GameLiving owner)
		{
			IControlledBrain controlledBrain = null;
			BdSubPet.SubPetType type = (BdSubPet.SubPetType)(byte)this.Spell.DamageType;
			owner = owner.ControlledBrain.Body;

			switch (type)
			{
				//Melee
				case BdSubPet.SubPetType.Melee:
					controlledBrain = new BdMeleeBrain(owner);
					break;
				//Healer
				case BdSubPet.SubPetType.Healer:
					controlledBrain = new BdHealerBrain(owner);
					break;
				//Mage
				case BdSubPet.SubPetType.Caster:
					controlledBrain = new BdCasterBrain(owner);
					break;
				//Debuffer
				case BdSubPet.SubPetType.Debuffer:
					controlledBrain = new BbDebufferBrain(owner);
					break;
				//Buffer
				case BdSubPet.SubPetType.Buffer:
					controlledBrain = new BdBufferBrain(owner);
					break;
				//Range
				case BdSubPet.SubPetType.Archer:
					controlledBrain = new BdArcherBrain(owner);
					break;
				//Other
				default:
					controlledBrain = new ControlledNpcBrain(owner);
					break;
			}

			return controlledBrain;
		}

		protected override GameSummonedPet GetGamePet(INpcTemplate template)
		{
			return new BdSubPet(template);
		}

		protected override void SetBrainToOwner(IControlledBrain brain)
		{
			Caster.ControlledBrain.Body.AddControlledNpc(brain);
		}

		/// <summary>
		/// Delve Info
		/// </summary>
		public override IList<string> DelveInfo
		{
			get
			{
				var delve = new List<string>();
                delve.Add(String.Format(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "SummonMinionHandler.DelveInfo.Text1", Spell.Target)));
                delve.Add(String.Format(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "SummonMinionHandler.DelveInfo.Text2", Math.Abs(Spell.Power))));
                delve.Add(LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "SummonMinionHandler.DelveInfo.Text3", (Spell.CastTime / 1000).ToString("0.0## " + LanguageMgr.GetTranslation((Caster as GamePlayer).Client, "Effects.DelveInfo.Seconds"))));
				return delve;
			}
		}
	}
}
