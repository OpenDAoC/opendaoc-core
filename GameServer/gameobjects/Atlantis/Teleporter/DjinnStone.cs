using System;
using log4net;
using System.Reflection;

namespace DOL.GS
{
    /// <summary>
    /// Djinn stone base class.
    /// </summary>
    /// <author>Aredhel</author>
    public class DjinnStone : GameStaticItem
    {
        private AncientBoundDjinn m_djinn;
        
        /// <summary>
        /// The djinn bound to this stone.
        /// </summary>
        protected AncientBoundDjinn Djinn
        {
            get { return m_djinn;  }
            set { m_djinn = value; }
        }
    }
}
