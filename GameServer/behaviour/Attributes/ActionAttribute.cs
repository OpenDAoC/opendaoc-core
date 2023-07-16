using System;
using System.Collections.Generic;
using System.Text;

namespace DOL.GS.Behaviour.Attributes
{    

    [AttributeUsage(AttributeTargets.Class)]
    public class ActionAttribute :Attribute
    {

        private eActionType actionType;

        public eActionType ActionType
        {
            get { return actionType; }
            set { actionType = value; }
        }

        private bool isNullableP;

        public bool IsNullableP
        {
            get { return isNullableP; }
            set { isNullableP = value; }
        }

        private bool isNullableQ;

        public bool IsNullableQ
        {
            get { return isNullableQ; }
            set { isNullableQ = value; }
        }

        private Object defaultValueP;

        public Object DefaultValueP
        {
            get { return defaultValueP; }
            set { defaultValueP = value; }
        }

        private Object defaultValueQ;

        public Object DefaultValueQ
        {
            get { return defaultValueQ; }
            set { defaultValueQ = value; }
        }
        
    }
}
