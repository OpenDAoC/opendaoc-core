using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.GS.Enums;
using Core.GS.Keeps;

namespace Core.GS.Quests
{
	public class ScoutMission : AMission
	{
		private AGameKeep m_keep = null;

		public ScoutMission(object owner)
			: base(owner)
		{
			ERealm realm = 0;
			if (owner is GroupUtil)
				realm = (owner as GroupUtil).Leader.Realm;
			else if (owner is GamePlayer)
				realm = (owner as GamePlayer).Realm;

			ArrayList list = new ArrayList();

			ICollection<AGameKeep> keeps;
			if (owner is GroupUtil)
				keeps = GameServer.KeepManager.GetKeepsOfRegion((owner as GroupUtil).Leader.CurrentRegionID);
			else if (owner is GamePlayer)
				keeps = GameServer.KeepManager.GetKeepsOfRegion((owner as GamePlayer).CurrentRegionID);
			else keeps = new List<AGameKeep>();

			foreach (AGameKeep keep in keeps)
			{
				if (keep.IsPortalKeep)
					continue;
				if (keep.Realm != realm)
					list.Add(keep);
			}

			if (list.Count > 0)
				m_keep = list[Util.Random(list.Count - 1)] as AGameKeep;

			GameEventMgr.AddHandler(AreaEvent.PlayerEnter, new CoreEventHandler(Notify));
			GameEventMgr.AddHandler(KeepEvent.KeepTaken, new CoreEventHandler(Notify));
		}

		public override void Notify(CoreEvent e, object sender, EventArgs args)
		{
			if (e == AreaEvent.PlayerEnter)
			{
				AreaEventArgs kargs = args as AreaEventArgs;

				if (m_owner is GamePlayer && kargs.GameObject != m_owner)
					return;

				foreach (AbstractArea area in kargs.GameObject.CurrentAreas)
				{
					if (area is KeepArea && (area as KeepArea).Keep == m_keep)
					{
						FinishMission();
						break;
					}
				}
			}
			else if (e == KeepEvent.KeepTaken)
			{
				KeepEventArgs kargs = args as KeepEventArgs;

				if (kargs.Keep != m_keep)
					return;

				ExpireMission();
			}
		}

		public override void FinishMission()
		{
			base.FinishMission();
			GameEventMgr.RemoveHandler(AreaEvent.PlayerEnter, new CoreEventHandler(Notify));
			GameEventMgr.RemoveHandler(KeepEvent.KeepTaken, new CoreEventHandler(Notify));
		}

		public override void ExpireMission()
		{
			base.ExpireMission();
			GameEventMgr.RemoveHandler(AreaEvent.PlayerEnter, new CoreEventHandler(Notify));
			GameEventMgr.RemoveHandler(KeepEvent.KeepTaken, new CoreEventHandler(Notify));
		}

		public override string Description
		{
			get
			{
				if (m_keep == null)
					return "Keep is null when trying to send the description";
				else return "Scout the area around " + m_keep.Name;
			}
		}

		public override long RewardRealmPoints
		{
			get
			{
				return 250;
			}
		}
	}
}