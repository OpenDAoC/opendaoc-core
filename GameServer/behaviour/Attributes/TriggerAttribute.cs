using System;
using System.Collections.Generic;
using System.Text;

namespace DOL.GS.Behaviour.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TriggerAttribute :Attribute
    {

        private bool global;

        /// <summary>
        /// Define wether the trigger is registered globaly (doesn't depends on implicity quests notifying of player etc or not
        /// </summary>
        public bool Global
        {
            get { return global; }
            set { global = value; }
        }
	

        private eTriggerType triggerType;

        public eTriggerType TriggerType
        {
            get { return triggerType; }
            set { triggerType = value; }
        }

        private bool isNullableK;

        public bool IsNullableK
        {
            get { return isNullableK; }
            set { isNullableK = value; }
        }

        private bool isNullableI;

        public bool IsNullableI
        {
            get { return isNullableI; }
            set { isNullableI = value; }
        }

        private Object defaultValueK;

        public Object DefaultValueK
        {
            get { return defaultValueK; }
            set { defaultValueK = value; }
        }

        private Object defaultValueI;

        public Object DefaultValueI
        {
            get { return defaultValueI; }
            set { defaultValueI = value; }
        }
	
    }
}
