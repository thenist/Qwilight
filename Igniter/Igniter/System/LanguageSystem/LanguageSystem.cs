using Igniter.Properties;
using Igniter.Utilities;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Igniter
{
    public sealed partial class LanguageSystem
    {
        public static readonly LanguageSystem Instance = new LanguageSystem();

        public LanguageSystem()
        {
            var language = Utility.GetLanguage(CultureInfo.CurrentUICulture.LCID);
            var languageSystem = typeof(LanguageSystem);
            foreach (var property in languageSystem.GetProperties().Where(property => property.PropertyType == typeof(string)))
            {
                property.SetValue(this, default);
            }
            var textHeight = 0;
            var lastLanguage = string.Empty;
            PropertyInfo lastPropertyInfo = null;
            var defaultValue = string.Empty;
            var r = new Utf8JsonReader(Resources.Language);
            while (r.Read())
            {
                if (r.TokenType == JsonTokenType.StartObject)
                {
                    if (++textHeight == 2)
                    {
                        defaultValue = string.Empty;
                    }
                    continue;
                }

                if (r.TokenType == JsonTokenType.EndObject)
                {
                    if (--textHeight == 1 && lastPropertyInfo != null && string.IsNullOrEmpty(lastPropertyInfo.GetValue(this) as string))
                    {
                        lastPropertyInfo?.SetValue(this, defaultValue);
                    }
                    continue;
                }

                if (textHeight == 1)
                {
                    if (r.TokenType == JsonTokenType.PropertyName)
                    {
                        lastPropertyInfo = languageSystem.GetProperty(r.GetString());
                        continue;
                    }
                }

                if (textHeight == 2)
                {
                    switch (r.TokenType)
                    {
                        case JsonTokenType.PropertyName:
                            lastLanguage = r.GetString();
                            break;
                        case JsonTokenType.String:
                            if (lastLanguage == "en-US")
                            {
                                defaultValue = r.GetString();
                            }
                            if (lastLanguage == language)
                            {
                                lastPropertyInfo?.SetValue(this, r.GetString());
                            }
                            break;
                    }
                }
            }
        }
    }
}
