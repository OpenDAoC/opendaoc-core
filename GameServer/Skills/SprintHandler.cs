using System;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;

namespace DOL.GS.SkillHandler
{
	/// <summary>
	/// Handler for Sprint Ability clicks
	/// </summary>
	[SkillHandlerAttribute(Abilities.Sprint)]
	public class SprintHandler : IAbilityActionHandler
	{
		public void Execute(AbilityUtil ab, GamePlayer player)
		{
			player.Sprint(!player.IsSprinting);
		}
	}
}
