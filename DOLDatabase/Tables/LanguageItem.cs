
using DOL.Database.Attributes;

namespace DOL.Database
{
    // data table attribute not set until item translations are supported.
    class LanguageItem : LanguageDataObject
    {
        public override eTranslationIdentifier TranslationIdentifier
        {
            get { return eTranslationIdentifier.eItem; }
        }
    }
}