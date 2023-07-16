/* Created by Schaf
 * Last modified by Schaf on 10.12.2004 20:09
 */

using DOL.Database;

//yeah for the DBIndoorItem class!

namespace DOL.GS.Housing
{
	public class IndoorItem
	{
		public IndoorItem()
		{}

		public IndoorItem(DBHouseIndoorItem dbitem)
		{
			Model = dbitem.Model;
			Color = dbitem.Color;
			X = (short) dbitem.X;
			Y = (short) dbitem.Y;
			Rotation = dbitem.Rotation;
			Size = dbitem.Size;
			Emblem = dbitem.Emblem;
			Position = dbitem.Position;
			PlacementMode = dbitem.Placemode;
			BaseItem = GameServer.Database.FindObjectByKey<ItemTemplate>(dbitem.BaseItemID);
			DatabaseItem = dbitem;
		}

		public int Model { get; set; }

		public int Color { get; set; }

		public short X { get; set; }

		public short Y { get; set; }

		public int Rotation { get; set; }

		public int Size { get; set; }

		public int Emblem { get; set; }

		public int Position { get; set; }

		public int PlacementMode { get; set; }

		public ItemTemplate BaseItem { get; set; }

		public DBHouseIndoorItem DatabaseItem { get; set; }

		public DBHouseIndoorItem CreateDBIndoorItem(int houseNumber)
		{
			var dbitem = new DBHouseIndoorItem
			             	{
			             		HouseNumber = houseNumber,
			             		Model = Model,
			             		Position = Position,
			             		Placemode = PlacementMode,
			             		X = X,
			             		Y = Y
			             	};

			if (BaseItem != null)
			{
				dbitem.BaseItemID = BaseItem.Id_nb;
			}
			else
			{
				dbitem.BaseItemID = "null";
			}

			dbitem.Color = Color;
			dbitem.Emblem = Emblem;
			dbitem.Rotation = Rotation;
			dbitem.Size = Size;

			return dbitem;
		}
	}
}