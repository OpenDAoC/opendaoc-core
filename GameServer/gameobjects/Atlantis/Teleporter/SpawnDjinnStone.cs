using System;
using System.Collections.Generic;
using System.Text;

namespace DOL.GS
{
    /// <summary>
    /// Djinn stone (spawns ancient bound djinn).
    /// </summary>
    /// <author>Aredhel</author>
    public class SpawnDjinnStone : DjinnStone
    {
        /// <summary>
        /// Spawns the djinn as soon as the stone is added to
        /// the world.
        /// </summary>
        /// <returns></returns>
        public override bool AddToWorld()
        {
            if (Djinn == null)
                Djinn = new PermanentDjinn(this);

            return base.AddToWorld();
        }
    }
}
