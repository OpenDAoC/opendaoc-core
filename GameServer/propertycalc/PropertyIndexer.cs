using System;

namespace DOL.GS.PropertyCalc
{
	/// <summary>
	/// helper class for memory efficient usage of property fields
	/// it keeps integer values indexed by integer keys
	/// </summary>
	public sealed class PropertyIndexer : IPropertyIndexer
	{
		private readonly ReaderWriterDictionary<int, int> m_propDict;
		
		public PropertyIndexer()
		{
			m_propDict = new ReaderWriterDictionary<int, int>();
		}
		
		public PropertyIndexer(int fixSize)
		{
			m_propDict = new ReaderWriterDictionary<int, int>(fixSize);
		}
		
		public int this[int index]
		{
			get
			{
				int val;
				if (m_propDict.TryGetValue(index, out val))
					return val;
				return 0;
			}
			set
			{
				m_propDict[index] = value;
			}
		}
		
		public int this[eProperty index]
		{
			get
			{
				return this[(int)index];
			}
			set
			{
				this[(int)index] = value;
			}
		}
	}
}
