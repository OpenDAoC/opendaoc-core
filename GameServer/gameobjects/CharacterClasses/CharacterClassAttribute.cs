using System;
using System.Collections;
using System.Collections.Generic;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS
{
	/// <summary>
	/// Denotes a class as a DOL Character class
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class CharacterClassAttribute : Attribute
	{
		protected string m_name;
		protected string m_femaleName;
		protected string m_basename;
		protected int m_id;

		public CharacterClassAttribute(int id, string name, string basename, string femalename)
		{
			m_basename = basename;
			m_name = name;
			m_id = id;
			m_femaleName = femalename;
		}

		public CharacterClassAttribute(int id, string name, string basename)
		{
			m_basename = basename;
			m_name = name;
			m_id = id;
		}

		public int ID
		{
			get
			{
				return m_id;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
		}

		public string BaseName
		{
			get
			{
				return m_basename;
			}
		}

		public string FemaleName
		{
			get
			{
				return m_femaleName;
			}
		}
	}
}
