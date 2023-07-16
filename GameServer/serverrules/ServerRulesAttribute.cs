using System;

namespace DOL.GS.ServerRules
{
	/// <summary>
	/// Denotes a class as a server rules handler for a given server type
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ServerRulesAttribute : Attribute
	{
		protected eGameServerType m_serverType;

		public eGameServerType ServerType
		{
			get { return m_serverType; }
		}

		public ServerRulesAttribute(eGameServerType serverType)
		{
			m_serverType = serverType;
		}
	}
}
