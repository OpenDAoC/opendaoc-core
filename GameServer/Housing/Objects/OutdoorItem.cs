using DOL.Database;

namespace DOL.GS.Housing
{
	public class OutdoorItem
	{
		public OutdoorItem()
		{}

		public OutdoorItem(DbHouseOutdoorItems dbitem)
		{
			Model = dbitem.Model;
			Position = dbitem.Position;
			Rotation = dbitem.Rotation;
			BaseItem = GameServer.Database.FindObjectByKey<DbItemTemplates>(dbitem.BaseItemID);
			DatabaseItem = dbitem;
		}

		public int Model { get; set; }

		public int Position { get; set; }

		public int Rotation { get; set; }

		public DbItemTemplates BaseItem { get; set; }

		public DbHouseOutdoorItems DatabaseItem { get; set; }

		public DbHouseOutdoorItems CreateDBOutdoorItem(int houseNumber)
		{
			var dbitem = new DbHouseOutdoorItems
			             	{
			             		HouseNumber = houseNumber,
			             		Model = Model,
			             		Position = Position,
			             		BaseItemID = BaseItem.Id_nb,
			             		Rotation = Rotation
			             	};

			return dbitem;
		}
	}
}