﻿using System;
using System.Collections;
using DOL.GS.PacketHandler;
using DOL.AI.Brain;

namespace DOL.GS.Spells
{
	[SpellHandlerAttribute("PowerDrainPet")]
	public class PowerDrainPet : PowerDrain
	{
		public override void DrainPower(AttackData ad)
		{
			if ( !(m_caster is NecromancerPet))
				return;
			
			base.DrainPower(ad);
		}
		
		/// The power channelled through this spell goes to the owner, not the pet
		protected override GameLiving Owner()
		{
			return ((Caster as NecromancerPet).Brain as IControlledBrain).Owner;
		}
		
		/// <summary>
		/// Message the pet's owner, not the pet
		/// </summary>
		/// <param name="message"></param>
		/// <param name="chatType"></param>
		protected override void MessageToOwner(String message, eChatType chatType)
		{
			GameNPC npc = Caster as GameNPC;
			if (npc != null)
			{
				ControlledNpcBrain brain = npc.Brain as ControlledNpcBrain;
				if (brain != null)
				{
					GamePlayer owner = brain.Owner as GamePlayer;
					if (owner != null)
						owner.Out.SendMessage(message, chatType, eChatLoc.CL_SystemWindow);
				}
			}
		}
		
		/// <summary>
		/// Create a new handler for the necro petpower drain spell.
		/// </summary>
		/// <param name="caster"></param>
		/// <param name="spell"></param>
		/// <param name="line"></param>
		public PowerDrainPet(GameLiving caster, Spell spell, SpellLine line)
			: base(caster, spell, line) { }
	}
}