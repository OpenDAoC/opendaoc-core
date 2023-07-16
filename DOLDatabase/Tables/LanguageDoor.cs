
using DOL.Database.Attributes;

namespace DOL.Database
{
    // data table attribute not set until door translations are supported.
    class LanguageDoor : LanguageDataObject
    {
        public override eTranslationIdentifier TranslationIdentifier
        {
            get { return eTranslationIdentifier.eDoor; }
        }
    }
}