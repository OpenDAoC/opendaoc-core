using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using DOL.Events;
using DOL.Language;
using DOL.GS.PacketHandler;
using DOL.Database;
using DOL.GS.Spells;
using DOL.GS.Effects;
using log4net;

namespace DOL.GS
{
        /// <summary>
        /// Items of this class will proc on GameKeepComponent and GameKeepDoors, checked for in GameLiving-CheckWeaponMagicalEffect
        /// Used for Bruiser, or any other item that can fire a proc on keep components.  Itemtemplates must be set to DOL.GS.GameSiegeItem
        /// in the classtype field
        /// </summary>
        public class GameSiegeItem : GameInventoryItem
        {
                private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

                private GameSiegeItem() { }

                public GameSiegeItem(ItemTemplate template)
                        : base(template)
                {
                }

                public GameSiegeItem(ItemUnique template)
                        : base(template)
                {
                }

                public GameSiegeItem(InventoryItem item)
                        : base(item)
                {
                }
        }
}
 