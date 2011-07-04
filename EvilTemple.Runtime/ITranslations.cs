namespace EvilTemple.Runtime
{
    public interface ITranslations
    {

        string this[string key] { get; }

        bool Exists(string key);

    }

    public static class TranslationExtension
    {
        public static string Translate(this string str)
        {
            return str.StartsWith("#") ? Services.Get<ITranslations>()[str.Substring(1)] : str;
        }
    }

}
