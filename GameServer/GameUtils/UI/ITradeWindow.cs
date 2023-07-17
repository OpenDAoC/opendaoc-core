using System.Collections;
using DOL.Database;

namespace DOL.GS
{
	/// <summary>
	/// Description r�sume de ITradeWindow.
	/// </summary>
	public interface ITradeWindow
	{
		ArrayList TradeItems { get; set;}
		ArrayList PartnerTradeItems { get; }

		long TradeMoney { get; set; }
		long PartnerTradeMoney { get; }

		GamePlayer Owner { get; }
		GamePlayer Partner { get; }

		int ItemsCount { get; }
		int PartnerItemsCount { get; }

		bool Repairing { get; set; }
		bool Combine { get; set; }
		
		bool AddItemToTrade(InventoryItem itemForTrade);
		void RemoveItemToTrade(InventoryItem itemToRemove);
		void AddMoneyToTrade(long money);
		
		bool AcceptTrade();
		void TradeUpdate();
	
		object Sync { get; }

		void CloseTrade();
	}
}
