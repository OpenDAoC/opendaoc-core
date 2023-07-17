using System;
using System.Linq;

using DOL.Database;
using DOL.Events;

namespace DOL.GS.Keeps
{
	/// <summary>
	/// A keepComponent
	/// </summary>
	public class GameKeepHookPoint : Point3D
	{
		public GameKeepHookPoint(int id, GameKeepComponent component)
		{
			m_index = id;
			m_component = component;
			m_hookpointTimer = new HookpointTimer(this, this.Component);
			this.X = component.X;
			this.Y = component.Y;
			this.Z = component.Z;
			this.Heading = component.Heading;
		}

		public GameKeepHookPoint(DbKeepHookPoints dbhookPoint, GameKeepComponent component)
		{
			double angle = component.Keep.Heading * ((Math.PI * 2) / 360); // angle*2pi/360;
			switch (component.ComponentHeading)
			{
				case 0:
					X = (int)(component.X + Math.Cos(angle) * dbhookPoint.X + Math.Sin(angle) * dbhookPoint.Y);
					Y = (int)(component.Y - Math.Cos(angle) * dbhookPoint.Y + Math.Sin(angle) * dbhookPoint.X);
					break;
				case 1:
					X = (int)(component.X + Math.Cos(angle) * dbhookPoint.Y - Math.Sin(angle) * dbhookPoint.X);
					Y = (int)(component.Y + Math.Cos(angle) * dbhookPoint.X + Math.Sin(angle) * dbhookPoint.Y);
					break;
				case 2:
					X = (int)(component.X - Math.Cos(angle) * dbhookPoint.X - Math.Sin(angle) * dbhookPoint.Y);
					Y = (int)(component.Y + Math.Cos(angle) * dbhookPoint.Y - Math.Sin(angle) * dbhookPoint.X);
					break;
				case 3:
					X = (int)(component.X - Math.Cos(angle) * dbhookPoint.Y + Math.Sin(angle) * dbhookPoint.X);
					Y = (int)(component.Y - Math.Cos(angle) * dbhookPoint.X - Math.Sin(angle) * dbhookPoint.Y);
					break;
			}
			this.Z = component.Z + dbhookPoint.Z;
			this.Heading = (ushort)(component.Heading + dbhookPoint.Heading);
			this.m_index = dbhookPoint.HookPointID;
			this.Component = component;
			m_hookpointTimer = new HookpointTimer(this, this.Component);
		}

		#region properties

		// id <0x20=red,>0x20 - blue,>0x40 - green and yellow: 0x41(ballista),0x61(trebuchet),0x81(cauldron)
		private int m_index;
		public int ID
		{
			get { return m_index; }
			set { m_index = value; }
		}
		private HookpointTimer m_hookpointTimer;
		private GameKeepComponent m_component;
		public GameKeepComponent Component
		{
			get { return m_component; }
			set { m_component = value; }
		}

		public bool IsFree
		{
			get { return (m_object == null); }
		}

		private ushort m_heading;
		public ushort Heading
		{
			get { return m_heading; }
			set { m_heading = value; }
		}
		private GameLiving m_object;

		public GameLiving Object
		{
			get { return m_object; }
			set
			{
				m_object = value;
				if (value != null)
				{
					m_hookpointTimer.Start(1800000);//30*60*1000 = 30 min
					GameEventMgr.AddHandler(value, GameLivingEvent.Dying, new CoreEventHandler(ObjectDie));
				}
			}
		}

		#endregion

		private void ObjectDie(CoreEvent e, object sender, EventArgs arguments)
		{
			m_hookpointTimer.Start(300000);//5*60*1000 = 5 min
			GameEventMgr.RemoveHandler(m_object, GameLivingEvent.Dying, new CoreEventHandler(ObjectDie));
			var item = CoreDb<DBKeepHookPointItem>.SelectObject(DB.Column("KeepID").IsEqualTo(Component.Keep.KeepID).And(DB.Column("ComponentID").IsEqualTo(Component.ID)).And(DB.Column("HookPointID").IsEqualTo(ID)));
			if (item != null)
				GameServer.Database.DeleteObject(item);
		}
	}

	public class HookpointTimer : RegionECSAction
	{
		private GameKeepHookPoint m_hookpoint;

		public HookpointTimer(GameKeepHookPoint hookpoint, GameKeepComponent component)
			: base(component)
		{
			m_hookpoint = hookpoint;
		}

		protected override int OnTick(ECSGameTimer timer)
		{
			if (m_hookpoint.Object is GameSiegeWeapon)
				(m_hookpoint.Object as GameSiegeWeapon).ReleaseControl();
			if (m_hookpoint.Object.ObjectState != GameObject.eObjectState.Deleted)
			{
				m_hookpoint.Object.Delete();
				return 300000;//5*60*1000 = 5 min
			}
			else
				m_hookpoint.Object = null;

			return 0;
		}
	}
}