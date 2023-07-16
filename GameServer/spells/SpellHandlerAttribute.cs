using System;

namespace DOL.GS.Spells
{
	/// <summary>
	/// denotes a class as a spelltype handler for given spell type
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class SpellHandlerAttribute : Attribute
	{
		string m_type;

		public SpellHandlerAttribute(string spellType) {
			m_type = spellType;
		}

		/// <summary>
		/// Spell type name of the denoted handler
		/// </summary>
		public string SpellType {
			get { return m_type; }
		}
	}
}
