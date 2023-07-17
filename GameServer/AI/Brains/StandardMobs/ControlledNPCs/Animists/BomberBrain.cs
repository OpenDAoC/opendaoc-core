using DOL.Events;
using DOL.GS;

namespace DOL.AI.Brain
{
	public class BomberBrain : ControlledNpcBrain
	{
		const string BOMBERSPAWNTICK = "bomberspawntick";

		public BomberBrain(GameLiving owner) : base(owner) { }

		public override int ThinkInterval
		{
			get { return 700; }
		}

		protected override bool CheckDefensiveSpells(Spell spell)
		{
			return true;
		}

		protected override bool CheckOffensiveSpells(Spell spell)
		{
			return true;
		}

		#region Think
		public override void Think()
		{
			var spawnTick = Body.TempProperties.getProperty<long>(BOMBERSPAWNTICK);

			if (GameLoop.GameLoopTime - spawnTick > 60 * 1000)
			{
				Body.Delete();
			}
			
			GameLiving living = Body.TempProperties.getProperty<object>("bombertarget", null) as GameLiving;
			if(living == null) return;
			if(Body.IsWithinRadius( living, 150 ))
			{
				Body.Notify(GameNPCEvent.ArriveAtTarget, Body);
			}
		}
		
		/// <summary>
		/// Don't follow owner
		/// </summary>
		public override void FollowOwner() { }
		#endregion

		/// <summary>
		/// Updates the pet window
		/// </summary>
		public override void UpdatePetWindow() { }
	}
}