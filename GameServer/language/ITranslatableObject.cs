
using DOL.Database;

namespace DOL.Language
{
    public interface ITranslatableObject
    {
        string TranslationId { get; set; }

        LanguageDataObject.ETranslationIdentifier TranslationIdentifier { get; }
    }
}