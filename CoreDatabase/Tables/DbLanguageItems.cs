namespace DOL.Database
{
    // data table attribute not set until item translations are supported.
    class DbLanguageItems : LanguageDataObject
    {
        public override ETranslationIdentifier TranslationIdentifier
        {
            get { return ETranslationIdentifier.Item; }
        }
    }
}