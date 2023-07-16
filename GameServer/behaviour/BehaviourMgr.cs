using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using DOL.Database;
using DOL.Events;
using DOL.GS.PacketHandler;
using log4net;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour
{				
	/// <summary>
	/// Declares the behaviours managed, all behaviourtypes instances
	/// must be registered here to be usable
	/// </summary>
    public sealed class BehaviourMgr
    {  
		#region Declaration

		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		
        private static readonly IDictionary m_behaviourActionMap = new HybridDictionary();
        private static readonly IDictionary m_behaviourTriggerMap = new HybridDictionary();
        private static readonly IDictionary m_behaviourRequirementMap = new HybridDictionary();

		#endregion

        public static bool Init()
        {
            //We will search our assemblies for Quests by reflection so 
            //it is not neccessary anymore to register new quests with the 
            //server, it is done automatically!
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Walk through each type in the assembly
                foreach (Type type in assembly.GetTypes())
                {
                    // Pick up a class
                    if (type.IsClass != true)
                        continue;                                        
                    
                    if (typeof(IBehaviourAction).IsAssignableFrom(type))
                    {                        
                        ActionAttribute attr = GetActionAttribute(type);
                        if (attr != null)
                        {
                            if (log.IsInfoEnabled)
                                log.Info("Registering BehaviourAction: " + type.FullName);
                            RegisterBehaviourAction(attr.ActionType, type);
                        }
                    }

                    if (typeof(IBehaviourTrigger).IsAssignableFrom(type))
                    {
                        
                        TriggerAttribute attr = getTriggerAttribute(type);
                        if (attr != null)
                        {
                            if (log.IsInfoEnabled)
                                log.Info("Registering BehaviourTrigger: " + type.FullName);
                            RegisterBehaviourTrigger(attr.TriggerType, type);
                        }
                    }

                    if (typeof(IBehaviourRequirement).IsAssignableFrom(type))
                    {
                        
                        RequirementAttribute attr = getRequirementAttribute(type);
                        if (attr != null)
                        {
                            if (log.IsInfoEnabled)
                                log.Info("Registering BehaviourRequirement: " + type.FullName);
                            RegisterBehaviourRequirement(attr.RequirementType, type);
                        }
                    }
                }
            }
            return true;
        }

        public static ActionAttribute GetActionAttribute(Type type)
        {
            foreach (Attribute attr in type.GetCustomAttributes(typeof(ActionAttribute), false))
            {
                return (ActionAttribute)attr;
            }
            return null;
        }

        public static TriggerAttribute getTriggerAttribute(Type type)
        {
            foreach (Attribute attr in type.GetCustomAttributes(typeof(TriggerAttribute), false))
            {                
                return (TriggerAttribute)attr;
            }
            return null;
        }

        public static RequirementAttribute getRequirementAttribute(Type type)
        {
            foreach (Attribute attr in type.GetCustomAttributes(typeof(RequirementAttribute),false))
            {            
                return (RequirementAttribute)attr;
            }
            return null;
        }		
		
		/// <summary>
        /// Creates a new BehaviourBuilder
		/// </summary>		
        /// <returns>BehaviourBuilder</returns>
        public static BehaviourBuilder getBuilder() 
		{
            return new BehaviourBuilder();
        }

        public static void RegisterBehaviourAction(eActionType actionType, Type type)
        {
            if (m_behaviourActionMap.Contains(actionType))
            {
                if (log.IsErrorEnabled)
                    log.Error(actionType + " is already registered, only one Type can be declared for each ActionType. Duplicate declaration found in :" + m_behaviourActionMap[actionType] + " and " + type);
                return;
            }
            m_behaviourActionMap.Add(actionType, type);
        }

        public static Type GetTypeForActionType(eActionType actionType)
        {
            return (Type) m_behaviourActionMap[actionType];
        }

        public static void RegisterBehaviourTrigger(eTriggerType triggerType, Type type)
        {
            if (m_behaviourTriggerMap.Contains(triggerType))
            {
                if (log.IsErrorEnabled)
                    log.Error(triggerType + " is already registered, only one Type can be declared for each TriggerType. Duplicate declaration found in :" + m_behaviourTriggerMap[triggerType] + " and " + type);
                return;
            }
            m_behaviourTriggerMap.Add(triggerType, type);
        }

        public static Type GetTypeForTriggerType(eTriggerType triggerType)
        {
            return (Type)m_behaviourTriggerMap[triggerType];
        }

        public static void RegisterBehaviourRequirement(eRequirementType requirementType, Type type)
        {
            if (m_behaviourRequirementMap.Contains(requirementType))
            {
                if (log.IsErrorEnabled)
                    log.Error(requirementType + " is already registered, only one Type can be declared for each Requirement. Duplicate declaration found in :" + m_behaviourRequirementMap[requirementType] + " and " + type);
                return;
            }
            m_behaviourRequirementMap.Add(requirementType, type);
        }

        public static Type GetTypeForRequirementType(eRequirementType requirementType)
        {
            return (Type)m_behaviourRequirementMap[requirementType];
        }
		
    }

    /// <summary>
    /// Helper Class to Flag parameters in the generic Definition as unused
    /// </summary>
    public class Unused
    {
    }
}
