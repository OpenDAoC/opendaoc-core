
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
        public override ETranslationIdentifier TranslationIdentifier
        {
            get { return ETranslationIdentifier.System; }
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