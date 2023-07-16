using System;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Sprint Ability clicks
	/// </summary>
	[SkillHandlerAttribute(Abilities.Sprint)]
	public class SprintAbilityHandler : IAbilityActionHandler
	{
		public void Execute(Ability ab, GamePlayer player)
		{
			player.Sprint(!player.IsSprinting);
		}
	}
}
