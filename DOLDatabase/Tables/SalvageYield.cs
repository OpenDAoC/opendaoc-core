using System;
using DOL.Database.Attributes;

namespace DOL.Database
{
	/// <summary>
	/// The salvage table
	/// </summary>
	[DataTable(TableName="SalvageYield")]
	public class SalvageYield : DataObject
	{
		public const string LEGACY_SALVAGE_ID = "legacy_salvage_donotdelete";

		private int m_id;
		private int m_objectType;
		private int m_salvageLevel;
		private string m_materialId_nb;
		private int m_count;
        private int m_realm;
		private string m_packageID;

		[PrimaryKey(AutoIncrement = true)]
		public int ID
		{
			get { return m_id; }
			set { m_id = value; }
		}

		/// <summary>
		/// Object type of item to salvage, 0 if not used
		/// </summary>
		[DataElement(AllowDbNull = false, Index = true)]
		public int ObjectType
		{
			get
			{
				return m_objectType;
			}
			set
			{
				Dirty = true;
				m_objectType = value;
			}
		}
		/// <summary>
		/// The salvage level, 0 if not used
		/// </summary>
		[DataElement(AllowDbNull = false, Index = true)]
		public int SalvageLevel
		{
			get
			{
				return m_salvageLevel;
			}
			set
			{
				Dirty = true;
				m_salvageLevel = value;
			}
		}

		/// <summary>
		/// Index of item to craft
		/// </summary>
		[DataElement(AllowDbNull = false)]
		public string MaterialId_nb
		{
			get
			{
				return m_materialId_nb;
			}
			set
			{
				Dirty = true;
				m_materialId_nb = value;
			}
		}

		/// <summary>
		/// Count of material to return, 0 if calculated in the code
		/// </summary>
		[DataElement(AllowDbNull = false)]
		public int Count
		{
			get
			{
				return m_count;
			}
			set
			{
				Dirty = true;
				m_count = value;
			}
		}

        /// <summary>
        /// Realm of item to salvage, 0 for all realms
        /// </summary>
        [DataElement(AllowDbNull = false, Index = true)]
        public int Realm
        {
            get
            {
                return m_realm;
            }
            set
            {
                Dirty = true;
                m_realm = value;
            }
        }

		/// <summary>
		/// PackageID / description of this entry
		/// </summary>
		[DataElement(AllowDbNull = true)]
		public string PackageID
		{
			get
			{
				return m_packageID;
			}
			set
			{
				Dirty = true;
				m_packageID = value;
			}
		}

	}
}
