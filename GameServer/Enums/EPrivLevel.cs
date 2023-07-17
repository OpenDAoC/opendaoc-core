
namespace DOL.GS
{
    /// <summary>
    /// The privilege level of the client
    /// </summary>
    public enum EPrivLevel : uint
	{
		/// <summary>
		/// Normal player
		/// </summary>
		Player = 1,
		/// <summary>
		/// A GM
		/// </summary>
		GM = 2,
		/// <summary>
		/// An Admin
		/// </summary>
		Admin = 3,
	}
}
