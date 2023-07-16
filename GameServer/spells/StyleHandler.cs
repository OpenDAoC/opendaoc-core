﻿using System;
using System.Collections;
using System.Collections.Generic;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.GS.PacketHandler.Client.v168;
using DOL.GS.Styles;

namespace DOL.GS.Spells
{
	[SpellHandlerAttribute("StyleHandler")]
	public class StyleHandler : SpellHandler
	{
		public StyleHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }

		public override IList<string> DelveInfo
		{
			get
			{
				var list = new List<string>();
				list.Add(Spell.Description);

				GamePlayer player = Caster as GamePlayer;

				if (player != null)
				{
					list.Add(" ");

					Style style = SkillBase.GetStyleByID((int)Spell.Value, 0);
					if (style == null)
					{
						style = SkillBase.GetStyleByID((int)Spell.Value, player.CharacterClass.ID);
					}

					if (style != null)
					{
						DetailDisplayHandler.WriteStyleInfo(list, style, player.Client);
					}
					else
					{
						list.Add("Style not found.");
					}
				}

				return list;
			}
		}

	}


}

