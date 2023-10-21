using System;
using Core.Events;
using Core.GS.ECS;
using Core.GS.Events;

namespace Core.GS.RealmAbilities.Statics
{
	public abstract class GenericBase : GameStaticItem 
    {
		protected abstract string GetStaticName();
		protected abstract ushort GetStaticModel();
		protected abstract ushort GetStaticEffect();
		protected GamePlayer m_caster;
		protected uint m_lifeTime;
		protected ushort m_radius;
		protected uint m_pulseFrequency;
		int currentTick = 0;
		int currentPulse = 0;
		protected int getCurrentPulse ()
		{return currentPulse;}
		
		public void CreateStatic(GamePlayer caster, Point3D gt, uint lifeTime, uint pulseFrequency, ushort radius) 
        {
			m_lifeTime = lifeTime;
			m_caster = caster;
			m_radius = radius;
			m_pulseFrequency = pulseFrequency;
			this.Name = GetStaticName();
			this.Model = GetStaticModel();
			this.X = gt.X;
			this.Y = gt.Y;
			this.Z = gt.Z;
			this.CurrentRegionID = m_caster.CurrentRegionID;
			this.Level = caster.Level;
            this.Realm = caster.Realm;
            this.AddToWorld();
		}
		public override bool AddToWorld() 
        {
			new EcsGameTimer(this, new EcsGameTimer.EcsTimerCallback(PulseTimer),1000);
			GameEventMgr.AddHandler(m_caster, GamePlayerEvent.RemoveFromWorld, new CoreEventHandler(PlayerLeftWorld));
			return base.AddToWorld();
		}
		protected virtual int PulseTimer(EcsGameTimer timer)
        {
			if (currentTick >= m_lifeTime || m_caster == null) 
            {
				this.RemoveFromWorld();
				timer.Stop();
				timer=null;
				return 0;
			}
			if (currentTick%m_pulseFrequency==0){
				currentPulse++;
				foreach(GamePlayer target in this.GetPlayersInRadius(m_radius)) 
                {
					CastSpell(target);
				}
				foreach (GameNpc npc in this.GetNPCsInRadius(m_radius))	
                {
					CastSpell(npc);
				}
			}
			
			currentTick++;
			return 1000;
		}
		protected virtual void PlayerLeftWorld(CoreEvent e, object sender, EventArgs args)
		{
			GamePlayer player = (GamePlayer)sender;
			if (this.m_caster == player)
            {
				currentTick = (int)m_lifeTime;
			}
		}
		protected abstract void CastSpell (GameLiving target);		
	}
}