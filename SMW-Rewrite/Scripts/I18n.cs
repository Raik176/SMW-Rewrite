using System.Text;
using System.Text.RegularExpressions;

namespace SMW_Rewrite.Scripts {
    static internal class I18n {
        /// <summary>
        /// Represents the currently selected language.
        /// </summary>
        public static Language lang = Language.en_US;

        /// <summary>
        /// Cache for storing translated strings for different languages.
        /// </summary>
        private static readonly Dictionary<Language, Dictionary<string, string>> cache = new Dictionary<Language, Dictionary<string, string>>();

        /// <summary>
        /// Path to the directory containing translation files.
        /// </summary>
        private static readonly string i18nPath = "SMW_Rewrite.Assets.I18n.";

        /// <summary>
        /// Initializes the translation system by loading translations for the default language.
        /// </summary>
        /// <param name="lang">The default language to load translations for.</param>
        private static void InitLanguage(Language lang) {
            string curLang = i18nPath + lang.ToString() + ".i18n";
            Dictionary<string, string> temp = [];
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using Stream stream = assembly.GetManifestResourceStream(curLang);
            using StreamReader reader = new StreamReader(stream);
            string line, key, value, multiline = null, mlKey = null;
            while ((line = reader.ReadLine()) != null) {
                if (string.IsNullOrEmpty(line)) continue;
                if (multiline != null) {
                    if (line == "\"\"\"") {
                        temp[mlKey] = multiline.Substring(multiline.IndexOf("\n") + 1);
                        mlKey = null;
                        multiline = null;
                    } else multiline += "\n" + line;
                } else {
                    string[] keyValue = line.Split(new[] { '=' }, 2);
                    key = keyValue[0].Trim();
                    value = keyValue[1].Trim();
                    if (value == "\"\"\"") {
                        multiline = "";
                        mlKey = key;
                    } else temp[key] = value;
                }
            }

            cache.Add(lang, temp);
        }

        /// <summary>
        /// Static constructor. Initializes the translation system.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the 'key' parameter is invalid.</exception>
        static I18n() { // Initialize
            InitLanguage(lang);
        }

        /// <summary>
        /// Retrieves the translated string for the specified key in the default language, with no placeholders.
        /// </summary>
        /// <param name="key">The key corresponding to the translated string.</param>
        /// <returns>The translated string if found; otherwise, a placeholder indicating the key is missing.</returns>
        /// <exception cref="ArgumentException">Thrown when the 'key' parameter is invalid.</exception>
        public static string GetValue(string key) {
            return GetValueFrom(lang, key);
        }

        /// <summary>
        /// Retrieves the translated string for the specified key in the default language, with a default value and no placeholders.
        /// </summary>
        /// <param name="key">The key corresponding to the translated string.</param>
        /// <param name="defaultValue">The default value to return if the translation for the key is not found.</param>
        /// <returns>The translated string if found; otherwise, the default value.</returns>
        /// <exception cref="ArgumentException">Thrown when the 'key' parameter is invalid.</exception>
        public static string GetValue(string key, string defaultValue) {
            return GetValueFrom(lang, key, defaultValue);
        }

        /// <summary>
        /// Retrieves the translated string for the specified key in the given language, with no placeholders.
        /// </summary>
        /// <param name="language">The language to use for translation.</param>
        /// <param name="key">The key corresponding to the translated string.</param>
        /// <returns>The translated string if found; otherwise, a placeholder indicating the key is missing.</returns>
        /// <exception cref="ArgumentException">Thrown when the 'key' parameter is invalid.</exception>
        public static string GetValueFrom(Language language, string key) {
            return GetValueFromWith(language, key, null);
        }

        /// <summary>
        /// Retrieves the translated string for the specified key in the given language, with a default value and no placeholders.
        /// </summary>
        /// <param name="language">The language to use for translation.</param>
        /// <param name="key">The key corresponding to the translated string.</param>
        /// <param name="defaultValue">The default value to return if the translation for the key is not found.</param>
        /// <returns>The translated string if found; otherwise, the default value.</returns>
        /// <exception cref="ArgumentException">Thrown when the 'key' parameter is invalid.</exception>
        public static string GetValueFrom(Language language, string key, string defaultValue) {
            return GetValueFromWith(language, key, defaultValue, null);
        }

        /// <summary>
        /// Retrieves the translated string for the specified key in the default language, with placeholders replaced by the provided values.
        /// </summary>
        /// <param name="key">The key corresponding to the translated string.</param>
        /// <param name="placeholders">The values to replace placeholders in the translated string.</param>
        /// <returns>The translated string with placeholders replaced if found; otherwise, a placeholder indicating the key is missing.</returns>
        /// <exception cref="ArgumentException">Thrown when the 'key' parameter is invalid.</exception>
        public static string GetValueWith(string key, object[] placeholders) {
            return GetValueFromWith(lang, key, placeholders);
        }

        /// <summary>
        /// Retrieves the translated string for the specified key in the default language, with a default value and placeholders replaced by the provided values.
        /// </summary>
        /// <param name="key">The key corresponding to the translated string.</param>
        /// <param name="defaultValue">The default value to return if the translation for the key is not found.</param>
        /// <param name="placeholders">The values to replace placeholders in the translated string.</param>
        /// <returns>The translated string with placeholders replaced if found; otherwise, the default value.</returns>
        /// <exception cref="ArgumentException">Thrown when the 'key' parameter is invalid.</exception>
        public static string GetValueWith(string key, string defaultValue, object[] placeholders) {
            return GetValueFromWith(lang, key, defaultValue, placeholders);
        }

        /// <summary>
        /// Retrieves the translated string for the specified key in the given language, with placeholders replaced by the provided values.
        /// </summary>
        /// <param name="language">The language to use for translation.</param>
        /// <param name="key">The key corresponding to the translated string.</param>
        /// <param name="placeholders">The values to replace placeholders in the translated string.</param>
        /// <returns>The translated string with placeholders replaced if found; otherwise, a placeholder indicating the key is missing.</returns>
        /// <exception cref="ArgumentException">Thrown when the 'key' parameter is invalid.</exception>
        public static string GetValueFromWith(Language language, string key, object[] placeholders) {
            return GetValueFromWith(language, key, null, placeholders);
        }

        /// <summary>
        /// Retrieves the translated string for the specified key in the given language, with a default value and placeholders replaced by the provided values.
        /// </summary>
        /// <param name="language">The language to use for translation.</param>
        /// <param name="key">The key corresponding to the translated string.</param>
        /// <param name="defaultValue">The default value to return if the translation for the key is not found.</param>
        /// <param name="placeholders">The values to replace placeholders in the translated string.</param>
        /// <returns>The translated string with placeholders replaced if found; otherwise, the default value.</returns>
        /// <exception cref="ArgumentException">Thrown when the 'key' parameter is invalid.</exception>
        public static string GetValueFromWith(Language language, string key, string defaultValue, object[] placeholders) {
            if (string.IsNullOrWhiteSpace(key) || !Regex.IsMatch(key, @"^[a-zA-Z\.]+$")) throw new ArgumentException("The key must only contain dots or alphabetic characters and cannot be null or empty.", nameof(key));
            if (!cache.ContainsKey(language)) InitLanguage(language);
            string str = cache[language].GetValueOrDefault(key, string.IsNullOrEmpty(defaultValue) ? "Missing translation (" + key + ")" : defaultValue);
            if (placeholders != null) {
                for (int i = 0; i < placeholders.Length; i++) {
                    str = str.Replace("%" + i, placeholders[i].ToString());
                }
            }
            return str;
        }

        /// <summary>
        /// Retrieves the full text with placeholders replaced by translated values in the default language.
        /// </summary>
        /// <param name="text">The text containing placeholders to be replaced.</param>
        /// <returns>The text with placeholders replaced by translated values if found; otherwise, a placeholder indicating missing translations.</returns>
        /// <exception cref="ArgumentException">Thrown when any translation key is invalid.</exception>
        public static string GetFullText(string text) {
            return GetFullTextFrom(lang, text);
        }

        /// <summary>
        /// Retrieves the full text with placeholders replaced by translated values in the default language.
        /// </summary>
        /// <param name="text">The text containing placeholders to be replaced.</param>
        /// <returns>The text with placeholders replaced by translated values if found; otherwise, a placeholder indicating missing translations.</returns>
        /// <exception cref="ArgumentException">Thrown when any translation key is invalid.</exception>
        public static string GetFullTextWith(string text) {
            return GetFullTextFrom(lang, text);
        }

        /// <summary>
        /// Retrieves the full text with placeholders replaced by translated values in the specified language.
        /// </summary>
        /// <param name="language">The language to use for translation.</param>
        /// <param name="text">The text containing placeholders to be replaced.</param>
        /// <returns>The text with placeholders replaced by translated values if found; otherwise, a placeholder indicating missing translations.</returns>
        /// <exception cref="ArgumentException">Thrown when any translation key is invalid.</exception>
        public static string GetFullTextFrom(Language language, string text) {
            return GetFullTextFrom(language, text, null);
        }

        /// <summary>
        /// Retrieves the full text with placeholders replaced by translated values in the specified language.
        /// </summary>
        /// <param name="language">The language to use for translation.</param>
        /// <param name="text">The text, where translations are formatted like '%(key)'</param>
        /// <param name="defaultValue">The default value to use if translations for placeholders are not found.</param>
        /// <returns>The text with placeholders replaced by translated values if found; otherwise, a placeholder indicating missing translations.</returns>
        /// <exception cref="ArgumentException">Thrown when any translation key is invalid.</exception>
        public static string GetFullTextFrom(Language language, string text, string defaultValue) {
            string evaluator(Match match) {
                return GetValueFrom(language, match.Groups[1].Value, defaultValue);
            }

            return Regex.Replace(text, "%\\((.*?)\\)", evaluator);
        }

        /// <summary>
        /// Checks if the given key exists in the default language translations.
        /// </summary>
        /// <param name="key">The key to check for existence.</param>
        /// <returns>True if the key exists in translations; otherwise, false.</returns>
        public static bool HasKey(string key) {
            return HasKeyFrom(lang, key);
        }

        /// <summary>
        /// Checks if the given key exists in the translations for the specified language.
        /// </summary>
        /// <param name="language">The language to check for key existence.</param>
        /// <param name="key">The key to check for existence.</param>
        /// <returns>True if the key exists in translations; otherwise, false.</returns>
        public static bool HasKeyFrom(Language language, string key) {
            return cache[language].ContainsKey(key);
        }

        /// <summary>
        /// Enumeration representing various languages supported for translation.
        /// </summary>
        public enum Language {
            en,     // English
            es,     // Spanish
            fr,     // French
            de,     // German
            it,     // Italian
            pt,     // Portuguese
            ru,     // Russian
            zh,     // Chinese
            ja,     // Japanese
            ko,     // Korean
            ar,     // Arabic
            hi,     // Hindi
            bn,     // Bengali
            pa,     // Punjabi
            ur,     // Urdu
            vi,     // Vietnamese
            tr,     // Turkish
            fa,     // Persian
            nl,     // Dutch
            pl,     // Polish
            sv,     // Swedish
            da,     // Danish
            fi,     // Finnish
            no,     // Norwegian
            el,     // Greek
            he,     // Hebrew
            th,     // Thai
            id,     // Indonesian
            ms,     // Malay
            fil,    // Filipino

            en_US,  // English (United States)
            en_GB,  // English (United Kingdom)
            es_ES,  // Spanish (Spain)
            es_MX,  // Spanish (Mexico)
            fr_FR,  // French (France)
            fr_CA,  // French (Canada)
            de_DE,  // German (Germany)
            de_AT,  // German (Austria)
            de_CH,  // German (Switzerland)
            it_IT,  // Italian (Italy)
            pt_PT,  // Portuguese (Portugal)
            pt_BR,  // Portuguese (Brazil)
            ru_RU,  // Russian (Russia)
            zh_CN,  // Chinese (China)
            zh_TW,  // Chinese (Taiwan)
            ja_JP,  // Japanese (Japan)
            ko_KR,  // Korean (South Korea)
            ar_SA,  // Arabic (Saudi Arabia)
            hi_IN,  // Hindi (India)
            bn_BD,  // Bengali (Bangladesh)
            pa_IN,  // Punjabi (India)
            ur_PK,  // Urdu (Pakistan)
            vi_VN,  // Vietnamese (Vietnam)
            tr_TR,  // Turkish (Turkey)
            fa_IR,  // Persian (Iran)
            nl_NL,  // Dutch (Netherlands)
            pl_PL,  // Polish (Poland)
            sv_SE,  // Swedish (Sweden)
            da_DK,  // Danish (Denmark)
            fi_FI,  // Finnish (Finland)
            no_NO,  // Norwegian (Norway)
            el_GR,  // Greek (Greece)
            he_IL,  // Hebrew (Israel)
            th_TH,  // Thai (Thailand)
            id_ID,  // Indonesian (Indonesia)
            ms_MY,  // Malay (Malaysia)
            fil_PH, // Filipino (Philippines)
        }
    }
}
