
using DOL.Database;
using log4net;

namespace DOL.GS.DatabaseUpdate
{
    /// <summary>
    /// Checks and updates the ServerProperty table.
    /// </summary>
    [DatabaseUpdate]
    public class ServerPropertiesUpdate : IDatabaseUpdater
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Update()
        {
            RemoveACLKUNLS();
        }

        #region RemoveACLKUNLS
        /// <summary>
        /// Removes the no longer used 'allowed_custom_language_keys' and 'use_new_language_system' entries.
        /// </summary>
        private void RemoveACLKUNLS()
        {
            log.Info("Updating the ServerProperty table...");

            var properties = GameServer.Database.SelectAllObjects<DbServerProperties>();

            bool aclkFound = false;
            bool unlsFound = false;
            foreach (DbServerProperties property in properties)
            {
                if (property.Key != "allowed_custom_language_keys" && property.Key != "use_new_language_system")
                    continue;

                if (property.Key == "allowed_custom_language_keys")
                    aclkFound = true;

                if (property.Key == "use_new_language_system")
                    unlsFound = true;

                GameServer.Database.DeleteObject(property);

                if (aclkFound && unlsFound)
                    break;
            }

            log.Info("ServerProperty table update complete!");
        }
        #endregion RemoveACLKUNLS
    }
}