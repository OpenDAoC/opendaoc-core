/* Created by Schaf
 * Last modified by Schaf on 10.12.2004 20:09
 */

using DOL.Database;

//yeah for the DBOutdoorItem class!

namespace DOL.GS.Housing
{
	public class OutdoorItem
	{
		public OutdoorItem()
		{}

		public OutdoorItem(DBHouseOutdoorItem dbitem)
		{
			Model = dbitem.Model;
			Position = dbitem.Position;
			Rotation = dbitem.Rotation;
			BaseItem = GameServer.Database.FindObjectByKey<ItemTemplate>(dbitem.BaseItemID);
			DatabaseItem = dbitem;
		}

		public int Model { get; set; }

		public int Position { get; set; }

		public int Rotation { get; set; }

		public ItemTemplate BaseItem { get; set; }

		public DBHouseOutdoorItem DatabaseItem { get; set; }

		public DBHouseOutdoorItem CreateDBOutdoorItem(int houseNumber)
		{
			var dbitem = new DBHouseOutdoorItem
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