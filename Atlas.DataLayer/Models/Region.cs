﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.DataLayer.Models
{
    public class Region : DataObjectBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int Expansion { get; set; }
        public bool HousingEnabled { get; set; }
        public bool DivingEnabled { get; set; }
        public int WaterLevel { get; set; }
        public string ClassType { get; set; }
        public bool IsFontier { get; set; }

        public virtual ICollection<Zone> Zones { get; set; }
        public virtual ICollection<SpawnGroup> SpawnGroups { get; set; }
        public Region()
        {
            Zones = new HashSet<Zone>();
            SpawnGroups = new HashSet<SpawnGroup>();
        }
    }
}
