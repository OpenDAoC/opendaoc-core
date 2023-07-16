namespace DOL.Database.Transaction
{
	/// <summary>
	/// Connection isolation levels
	/// </summary>
	public enum IsolationLevel
	{
		DEFAULT,
		SERIALIZABLE,
	    REPEATABLE_READ,
		READ_COMMITTED,
	    READ_UNCOMMITTED,
		SNAPSHOT
	}
}