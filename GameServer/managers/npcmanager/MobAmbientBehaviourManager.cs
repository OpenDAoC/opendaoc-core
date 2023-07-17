﻿using System;
using System.Linq;
using System.Collections.Generic;
using DOL.Database;

namespace DOL.GS
{
    /// <summary>
    /// MobAmbientBehaviourManager handles Mob Ambient Behaviour Lazy Loading
    /// </summary>
    public sealed class MobAmbientBehaviourManager
    {
        /// <summary>
        /// Mob X Ambient Behaviour Cache indexed by Mob Name
        /// </summary>
        private Dictionary<string, DbMobXAmbientBehaviors[]> AmbientBehaviour { get; }

        /// <summary>
        /// Retrieve MobXambiemtBehaviour Objects from Mob Name
        /// </summary>
        public DbMobXAmbientBehaviors[] this[string index]
        {
            get
            {
                if (string.IsNullOrEmpty(index))
                {
                    return new DbMobXAmbientBehaviors[0];
                }

                var lower = index.ToLower();
                return AmbientBehaviour.ContainsKey(lower)
                    ? AmbientBehaviour[lower]
                    : new DbMobXAmbientBehaviors[0];
            }
        }

        /// <summary>
        /// Create a new Instance of <see cref="MobAmbientBehaviourManager"/>
        /// </summary>
        public MobAmbientBehaviourManager(IObjectDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            AmbientBehaviour = database.SelectAllObjects<DbMobXAmbientBehaviors>()
                .GroupBy(x => x.Source)
                .ToDictionary(key => key.Key.ToLower(), value => value.ToArray());
        }
    }
}
