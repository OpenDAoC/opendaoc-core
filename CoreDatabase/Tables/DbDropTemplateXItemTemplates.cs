
using DOL.Database.Attributes;

namespace DOL.Database
{
	/// <summary>
	/// 
	/// </summary>
	[DataTable(TableName = "DropTemplateXItemTemplate")]
	public class DbDropTemplateXItemTemplates : DbLootTemplates
	{
		public DbDropTemplateXItemTemplates()
		{
		}

		[PrimaryKey(AutoIncrement = true)]
		public long ID { get; set; }
	}
}
