using System;
using System.Collections;
using DOL.Database;


namespace DOL.GS.Keeps
{
	public interface IKeepItem
	{
		ushort CurrentRegionID { get;set;}
		int X { get;set;}
		int Y { get;set;}
		int Z { get;set;}
		ushort Heading { get;set;}
		string TemplateID { get;}
		GameKeepComponent Component { get; set;}
		DbKeepPositions Position { get;set;}
		void LoadFromPosition(DbKeepPositions position, GameKeepComponent component);
		void MoveToPosition(DbKeepPositions position);
	}
}