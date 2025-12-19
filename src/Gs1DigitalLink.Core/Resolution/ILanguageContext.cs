namespace Gs1DigitalLink.Core.Resolution;

public interface ILanguageContext
{
    IEnumerable<LanguagePreference> GetLanguages();
}
