using System;
using System.Collections;
using System.Reflection;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using DOL.Events;
using DOL.Database;

namespace DOL.GS.RealmAbilities
{
	public class MasteryofConcentrationAbility : TimedRealmAbility
	{
        public MasteryofConcentrationAbility(DBAbility dba, int level) : base(dba, level) { }
		public const Int32 Duration = 30 * 1000;

		public override void Execute(GameLiving living)
		{
			if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED)) return;
			GamePlayer caster = living as GamePlayer;

			if (caster == null)
				return;

			XMasteryofConcentrationEffect MoCEffect = caster.EffectList.GetOfType<XMasteryofConcentrationEffect>();
			if (MoCEffect != null)
			{
				MoCEffect.Cancel(false);
				return;
			}
			
			// Check for the RA5L on the Sorceror: he cannot cast MoC when the other is up
			ShieldOfImmunityEffect ra5l = caster.EffectList.GetOfType<ShieldOfImmunityEffect>();
			if (ra5l != null)
			{
				caster.Out.SendMessage("You cannot currently use this ability", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				return;
			}
			
			SendCasterSpellEffectAndCastMessage(living, 7007, true);
			foreach (GamePlayer player in caster.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
			{

                if ( caster.IsWithinRadius( player, WorldMgr.INFO_DISTANCE ) )
				{
					if (player == caster)
					{
						player.MessageToSelf("You cast " + this.Name + "!", eChatType.CT_Spell);
						player.MessageToSelf("You become steadier in your casting abilities!", eChatType.CT_Spell);
					}
					else
					{
						player.MessageFromArea(caster, caster.Name + " casts a spell!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
						player.Out.SendMessage(caster.Name + "'s castings have perfect poise!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
				}
			}

			DisableSkill(living);

			new XMasteryofConcentrationEffect(Duration).Start(caster);
		}
        public override int GetReUseDelay(int level)
        {
            return 600;
        }
        
        public virtual int GetAmountForLevel(int level)
		{
        	if(ServerProperties.Properties.USE_NEW_ACTIVES_RAS_SCALING)
        	{
        		switch(level)
        		{
        			case 1: return 25;
        			case 2: return 35;
        			case 3: return 50;
        			case 4: return 60;
        			case 5: return 75;
        		}
        	}
        	else
        	{
         		switch(level)
        		{
        			case 1: return 25;
        			case 2: return 50;
        			case 3: return 75;
        		}       		
        	}
        	return 25;
		}
	}
}
