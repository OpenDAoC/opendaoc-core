
using DOL.Database.Attributes;

namespace DOL.Database
{
    [DataTable(TableName = "LanguageSystem")]
    public class DBLanguageSystem : LanguageDataObject
    {
        #region Variables
        private string m_text = string.Empty;
        #endregion Variables

        public DBLanguageSystem()
            : base() { }


        #region Properties
        public override eTranslationIdentifier TranslationIdentifier
        {
            get { return eTranslationIdentifier.eSystem; }
        }

        [DataElement(AllowDbNull = false)]
        public string Text
        {
            get { return m_text; }
            set
            {
                Dirty = true;
                m_text = value;
            }
        }
        #endregion Properties
    }
}