using System;
using System.Collections.Generic;
using DOL.AI.Brain;
using DOL.Database;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.GS.RealmAbilities;
using DOL.GS.ServerProperties;
using DOL.Language;

namespace DOL.GS
{
	public class NecromancerPet : GameSummonedPet
	{
		public override GameObject TargetObject
		{
			get => base.TargetObject;
			set
			{
				// 1.60:
				// - A Necromancer's target window will now update to reflect a target his pet has acquired, if he does not already have a target.
				if (TargetObject != value && Owner is GamePlayer playerOwner && playerOwner.TargetObject == null)
					playerOwner.Client.Out.SendChangeTarget(value);
				base.TargetObject = value;
			}
		}

		public override long DamageRvRMemory
		{
			get => m_damageRvRMemory;
			set => m_damageRvRMemory = value;
		}

		/// <summary>
		/// Proc IDs for various pet weapons.
		/// </summary>
		private enum Procs
		{
			Cold = 32050,
			Disease = 32014,
			Heat = 32053,
			Poison = 32013,
			Stun = 2165
		};

		/// <summary>
		/// Create necromancer pet from template. Con and hit bonuses from
		/// items the caster was wearing when the summon started, will be
		/// transferred to the pet.
		/// </summary>
		/// <param name="npcTemplate"></param>
		/// <param name="owner">Player who summoned this pet.</param>
		/// <param name="summonConBonus">Item constitution bonuses of the player.</param>
		/// <param name="summonHitsBonus">Hits bonuses of the player.</param>
		public NecromancerPet(INpcTemplate npcTemplate, int summonConBonus, int summonHitsBonus) : base(npcTemplate)
		{
			// Transfer bonuses.
			m_summonConBonus = summonConBonus;
			m_summonHitsBonus = summonHitsBonus;

			// Set immunities/load equipment/etc.
			switch (Name.ToLower())
			{
				case "lesser zombie servant":
				case "zombie servant":
					EffectList.Add(new MezzRootImmunityEffect());
					LoadEquipmentTemplate("barehand_weapon");
					InventoryItem item;
					if (Inventory != null && (item = Inventory.GetItem(eInventorySlot.RightHandWeapon)) != null)
						item.ProcSpellID = (int)Procs.Stun;
					break;
				case "reanimated servant" :
					LoadEquipmentTemplate("reanimated_servant");
					break;
				case "necroservant":
					LoadEquipmentTemplate("necroservant");
					break;
				case "greater necroservant":
					LoadEquipmentTemplate("barehand_weapon");
					if (Inventory != null && (item = Inventory.GetItem(eInventorySlot.RightHandWeapon)) != null)
						item.ProcSpellID = (int)Procs.Poison;
					break;
				case "abomination":
					LoadEquipmentTemplate("abomination_fiery_sword");
					break;
				default:
					LoadEquipmentTemplate("barehand_weapon");
					break;
			}
		}

		#region Stats

		private int m_summonConBonus;
		private int m_summonHitsBonus;

		/// <summary>
		/// Get modified bonuses for the pet; some bonuses come from the shade, some come from the pet.
		/// </summary>
		public override int GetModified(EProperty property)
		{
			if (Brain == null || (Brain as IControlledBrain) == null)
				return base.GetModified(property);

			GameLiving livingOwner = (Brain as IControlledBrain).GetLivingOwner();
			GamePlayer playerOwner = livingOwner as GamePlayer;

			switch (property)
			{
				case EProperty.Resist_Body:
				case EProperty.Resist_Cold:
				case EProperty.Resist_Crush:
				case EProperty.Resist_Energy:
				case EProperty.Resist_Heat:
				case EProperty.Resist_Matter:
				case EProperty.Resist_Slash:
				case EProperty.Resist_Spirit:
				case EProperty.Resist_Thrust:
					return base.GetModified(property);
				case EProperty.Strength:
				case EProperty.Dexterity:
				case EProperty.Quickness:
				case EProperty.Intelligence:
					{
						// Get item bonuses from the shade, but buff bonuses from the pet.
						int itemBonus = livingOwner.GetModifiedFromItems(property);
						int buffBonus = GetModifiedFromBuffs(property);
						int debuff = DebuffCategory[(int)property];

						// Base stats from the pet; add this to item bonus
						// afterwards, as it is treated the same way for
						// debuffing purposes.
						int baseBonus = 0;
						int augRaBonus = 0;

						switch (property)
						{
							case EProperty.Strength:
								baseBonus = Strength;
								augRaBonus = OfRaHelpers.GetStatEnhancerAmountForLevel(playerOwner != null ? OfRaHelpers.GetAugStrLevel(playerOwner) : 0);
								break;
							case EProperty.Dexterity:
								baseBonus = Dexterity;
								augRaBonus = OfRaHelpers.GetStatEnhancerAmountForLevel(playerOwner != null ? OfRaHelpers.GetAugDexLevel(playerOwner) : 0);
								break;
							case EProperty.Quickness:
								baseBonus = Quickness;
								augRaBonus = OfRaHelpers.GetStatEnhancerAmountForLevel(playerOwner != null ? OfRaHelpers.GetAugQuiLevel(playerOwner) : 0);
								break;
							case EProperty.Intelligence:
								baseBonus = Intelligence;
								augRaBonus = OfRaHelpers.GetStatEnhancerAmountForLevel(playerOwner != null ? OfRaHelpers.GetAugAcuityLevel(playerOwner) : 0);
								break;
						}

						itemBonus += baseBonus + augRaBonus;

						// Apply debuffs. 100% Effectiveness for player buffs, but only 50% effectiveness for item bonuses.
						buffBonus -= Math.Abs(debuff);

						if (buffBonus < 0)
						{
							itemBonus += buffBonus / 2;
							buffBonus = 0;
							if (itemBonus < 0)
								itemBonus = 0;
						}

						return itemBonus + buffBonus;
					}
				case EProperty.Constitution:
					{
						int baseBonus = Constitution;
						int buffBonus = GetModifiedFromBuffs(EProperty.Constitution);
						int debuff = DebuffCategory[(int)property];

						// Apply debuffs. 100% Effectiveness for player buffs, but only 50% effectiveness for base bonuses.
						buffBonus -= Math.Abs(debuff);

						if (buffBonus < 0)
						{
							baseBonus += buffBonus / 2;
							buffBonus = 0;
							if (baseBonus < 0)
								baseBonus = 0;
						}

						return baseBonus + buffBonus;
					}
				case EProperty.MaxHealth:
					{
						int conBonus = (int)(3.1 * m_summonConBonus);
						int hitsBonus = 30 * Level + m_summonHitsBonus;
						int debuff = DebuffCategory[(int)property];

						// Apply debuffs. As only base constitution affects pet health, effectiveness is a flat 50%.
						conBonus -= Math.Abs(debuff) / 2;

						if (conBonus < 0)
							conBonus = 0;
						
						int totalBonus = conBonus + hitsBonus;

						OfRaToughnessHandler toughness = playerOwner?.GetAbility<OfRaToughnessHandler>();
						double toughnessMod = toughness != null ? 1 + toughness.GetAmountForLevel(toughness.Level) * 0.01 : 1;

						return (int)(totalBonus * toughnessMod);
					}
			}

			return base.GetModified(property);
		}

		public override int Health
		{
			get => base.Health;
			set
			{
				value = Math.Min(value, MaxHealth);
				value = Math.Max(value, 0);

				if (Health == value)
				{
					base.Health = value; // Needed to start regeneration.
					return;
				}

				int oldPercent = HealthPercent;
				base.Health = value;
				if (oldPercent != HealthPercent)
				{
					// Update pet health in group window.
					GamePlayer owner = (Brain as IControlledBrain).Owner as GamePlayer;
					owner.Group?.UpdateMember(owner, false, false);
				}
			}
		}

		/// <summary>
		/// Set stats according to necro pet server properties.
		/// </summary>
		public override void AutoSetStats(DbMobs dbMob = null)
		{
			int levelMinusOne = Level - 1;

			if (Name.ToUpper() == "GREATER NECROSERVANT")
			{
				Strength = ServerProperties.ServerProperties.NECRO_GREATER_PET_STR_BASE;
				Constitution = (short) (ServerProperties.ServerProperties.NECRO_GREATER_PET_CON_BASE + m_summonConBonus);
				Dexterity = ServerProperties.ServerProperties.NECRO_GREATER_PET_DEX_BASE;
				Quickness = ServerProperties.ServerProperties.NECRO_GREATER_PET_QUI_BASE;
				Intelligence = ServerProperties.ServerProperties.NECRO_GREATER_PET_INT_BASE;

				if (Level > 1)
				{
					Strength += (short) Math.Round(levelMinusOne * ServerProperties.ServerProperties.NECRO_GREATER_PET_STR_MULTIPLIER);
					Constitution += (short) Math.Round(levelMinusOne * ServerProperties.ServerProperties.NECRO_GREATER_PET_CON_MULTIPLIER);
					Dexterity += (short) Math.Round(levelMinusOne * ServerProperties.ServerProperties.NECRO_GREATER_PET_DEX_MULTIPLIER);
					Quickness += (short) Math.Round(levelMinusOne * ServerProperties.ServerProperties.NECRO_GREATER_PET_QUI_MULTIPLIER);
					Intelligence += (short) Math.Round(levelMinusOne * ServerProperties.ServerProperties.NECRO_GREATER_PET_INT_MULTIPLIER);
				}
			}
			else
			{
				Strength = ServerProperties.ServerProperties.NECRO_PET_STR_BASE;
				Constitution = (short) (ServerProperties.ServerProperties.NECRO_PET_CON_BASE + m_summonConBonus);
				Dexterity = ServerProperties.ServerProperties.NECRO_PET_DEX_BASE;
				Quickness = ServerProperties.ServerProperties.NECRO_PET_QUI_BASE;
				Intelligence = ServerProperties.ServerProperties.NECRO_PET_INT_BASE;

				if (Level > 1)
				{
					Strength += (short) Math.Round(levelMinusOne * ServerProperties.ServerProperties.NECRO_PET_STR_MULTIPLIER);
					Constitution += (short) Math.Round(levelMinusOne * ServerProperties.ServerProperties.NECRO_PET_CON_MULTIPLIER);
					Dexterity += (short) Math.Round(levelMinusOne * ServerProperties.ServerProperties.NECRO_PET_DEX_MULTIPLIER);
					Quickness += (short) Math.Round(levelMinusOne * ServerProperties.ServerProperties.NECRO_PET_QUI_MULTIPLIER);
					Intelligence += (short) Math.Round(levelMinusOne * ServerProperties.ServerProperties.NECRO_PET_INT_MULTIPLIER);
				}
			}

			Empathy = 30;
			Piety = 30;
			Charisma = 30;

			// Stats are scaled using the current template.
			if (NPCTemplate != null)
			{
				if (NPCTemplate.Strength > 0)
					Strength = (short) Math.Round(Strength * (NPCTemplate.Strength / 100.0));
				if (NPCTemplate.Constitution > 0)
					Constitution = (short) Math.Round(Constitution * (NPCTemplate.Constitution / 100.0));
				if (NPCTemplate.Quickness > 0)
					Quickness = (short) Math.Round(Quickness * (NPCTemplate.Quickness / 100.0));
				if (NPCTemplate.Dexterity > 0)
					Dexterity = (short) Math.Round(Dexterity * (NPCTemplate.Dexterity / 100.0));
				if (NPCTemplate.Intelligence > 0)
					Intelligence = (short) Math.Round(Intelligence * (NPCTemplate.Intelligence / 100.0));
				if (NPCTemplate.Empathy > 0)
					Empathy = NPCTemplate.Empathy;
				if (NPCTemplate.Piety > 0)
					Piety = NPCTemplate.Piety;
				if (NPCTemplate.Charisma > 0)
					Charisma = NPCTemplate.Charisma;
			}
		}

		#endregion

		#region Melee

		private void ToggleTauntMode()
		{
			TauntEffect tauntEffect = EffectList.GetOfType<TauntEffect>();
			GamePlayer owner = (Brain as IControlledBrain).Owner as GamePlayer;

			if (tauntEffect != null)
			{
				tauntEffect.Stop();
				owner.Out.SendMessage(string.Format("{0} seems to be less aggressive than before.", GetName(0, true)), EChatType.CT_System, EChatLoc.CL_SystemWindow);
			}
			else
			{
				owner.Out.SendMessage(string.Format("{0} enters an aggressive stance.", GetName(0, true)), EChatType.CT_System, EChatLoc.CL_SystemWindow);
				new TauntEffect().Start(this);
			}
		}

		#endregion

		#region Spells

		/// <summary>
		/// Pet-only insta spells.
		/// </summary>
		public static string PetInstaSpellLine => "Necro Pet Insta Spells";

		/// <summary>
		/// Called when necro pet is hit to see if spellcasting is interrupted.
		/// </summary>
		/// <param name="ad">information about the attack</param>
		public override void OnAttackedByEnemy(AttackData ad)
		{
			if (ad.AttackType == AttackData.EAttackType.Spell && ad.Damage > 0)
			{
				GamePlayer player = Owner as GamePlayer;
				string modmessage = "";

				if (ad.Modifier > 0)
					modmessage = " (+" + ad.Modifier + ")";
				else if (ad.Modifier < 0)
					modmessage = " (" + ad.Modifier + ")";

				player.Out.SendMessage(string.Format(LanguageMgr.GetTranslation(player.Client.Account.Language, "GameLiving.AttackData.HitsForDamage"), ad.Attacker.GetName(0, true), ad.Target.Name, ad.Damage, modmessage), EChatType.CT_Damaged, EChatLoc.CL_SystemWindow);

				if (ad.CriticalDamage > 0)
					player.Out.SendMessage(string.Format(LanguageMgr.GetTranslation(player.Client.Account.Language, "GameLiving.AttackData.CriticallyHitsForDamage"), ad.Attacker.GetName(0, true), ad.Target.Name, ad.CriticalDamage), EChatType.CT_Damaged, EChatLoc.CL_SystemWindow);
			}

			base.OnAttackedByEnemy(ad);
		}

		public override void ModifyAttack(AttackData attackData)
		{
			base.ModifyAttack(attackData);

			if ((Owner as GamePlayer).Client.Account.PrivLevel > (int)EPrivLevel.Player)
			{
				attackData.Damage = 0;
				attackData.CriticalDamage = 0;
			}
		}

		/// <summary>
		/// Taunt the current target.
		/// </summary>
		public void Taunt()
		{
			if (IsIncapacitated)
				return;

			SpellLine chantsLine = SkillBase.GetSpellLine("Chants");
			if (chantsLine == null)
				return;

			List<Spell> chantsList = SkillBase.GetSpellList("Chants");
			if (chantsList.Count == 0)
				return;

			// Find the best paladin taunt for this level.
			Spell tauntSpell = null;
			foreach (Spell spell in chantsList)
			{
				if (spell.SpellType == ESpellType.Taunt && spell.Level <= Level)
					tauntSpell = spell;
			}

			if (tauntSpell != null && GetSkillDisabledDuration(tauntSpell) == 0)
				CastSpell(tauntSpell, chantsLine);
		}

		#endregion

		public override bool SayReceive(GameLiving source, string str)
		{
			return WhisperReceive(source, str);
		}

		public override bool Interact(GamePlayer player)
		{
			return WhisperReceive(player, "arawn");
		}

		public override void TakeDamage(GameObject source, EDamageType damageType, int damageAmount, int criticalAmount)
		{
			criticalAmount /= 2;
			base.TakeDamage(source, damageType, damageAmount, criticalAmount);
		}

		/// <summary>
		/// Load equipment for the pet.
		/// </summary>
		/// <param name="templateID">Equipment Template ID.</param>
		/// <returns>True on success, else false.</returns>
		private bool LoadEquipmentTemplate(string templateID)
		{
			if (templateID.Length <= 0)
				return false;

			GameNpcInventoryTemplate inventoryTemplate = new();

			if (inventoryTemplate.LoadFromDatabase(templateID))
			{
				Inventory = new GameNpcInventory(inventoryTemplate);
				InventoryItem item;

				if ((item = Inventory.GetItem(eInventorySlot.TwoHandWeapon)) != null)
				{
					item.DPS_AF = (int)(Level * 3.3);
					item.SPD_ABS = 50;

					switch (templateID)
					{
						case "abomination_fiery_sword":
						case "abomination_flaming_mace":
							item.ProcSpellID = (int)Procs.Heat;
							break;
						case "abomination_icy_sword":
						case "abomination_frozen_mace":
							item.ProcSpellID = (int)Procs.Cold;
							break;
						case "abomination_poisonous_sword":
						case "abomination_venomous_mace":
							item.ProcSpellID = (int)Procs.Poison;
							break;
					}

					SwitchWeapon(EActiveWeaponSlot.TwoHanded);
				}
				else
				{
					if ((item = Inventory.GetItem(eInventorySlot.RightHandWeapon)) != null)
					{
						item.DPS_AF = (int)(Level * 3.3);
						item.SPD_ABS = 37;
					}

					if ((item = Inventory.GetItem(eInventorySlot.LeftHandWeapon)) != null)
					{
						item.DPS_AF = (int)(Level * 3.3);
						item.SPD_ABS = 37;
					}

					SwitchWeapon(EActiveWeaponSlot.Standard);
				}
			}

			foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
			{
				if (player == null)
					continue;

				player.Out.SendLivingEquipmentUpdate(this);
			}

			return true;
		}

		/// <summary>
		/// Pet stayed out of range for too long, despawn it.
		/// </summary>
		public void CutTether()
		{
			if ((Brain as IControlledBrain).Owner is not GamePlayer)
				return;

			Brain.Stop();
			Die(null);
		}
	}
}
