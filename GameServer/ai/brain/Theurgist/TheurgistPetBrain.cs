
using DOL.GS;

namespace DOL.AI.Brain
{
	public class TheurgistPetBrain : ControlledNpcBrain
	{
		private GameObject m_target;

		public TheurgistPetBrain(GameLiving owner) : base(owner)
		{
			IsMainPet = false;
		}

		public override void Think()
		{
			m_target = Body.TargetObject;

			if (m_target == null || m_target.Health <= 0)
			{
				Body.Die(null);
				return;
			}

			if (Body.FollowTarget != m_target)
			{
				Body.StopFollowing();
				Body.Follow(m_target, MIN_ENEMY_FOLLOW_DIST, MAX_ENEMY_FOLLOW_DIST);
			}

			if (!CheckSpells(eCheckSpellType.Offensive))
				Body.StartAttack(m_target);
		}

		public override eWalkState WalkState { get => eWalkState.Stay; set { } }
		public override eAggressionState AggressionState { get => eAggressionState.Aggressive; set { } }
		public override void Attack(GameObject target) { }
		public override void Disengage() { }
		public override void Follow(GameObject target) { }
		public override void FollowOwner() { }
		public override void Stay() { }
		public override void ComeHere() { }
		public override void Goto(GameObject target) { }
		public override void UpdatePetWindow() { }
	}
}
