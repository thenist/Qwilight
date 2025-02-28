﻿using Language;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

var wwwClient = new HttpClient();

var defaultJSONConfigure = new JsonSerializerOptions
{
    IncludeFields = true
};
var assetsClientJSON = JsonSerializer.Deserialize<JSON.Client>(await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "Assets", "Client.json")), defaultJSONConfigure);
wwwClient.DefaultRequestHeaders.Add("X-NCP-APIGW-API-KEY-ID", assetsClientJSON.nhnID);
wwwClient.DefaultRequestHeaders.Add("X-NCP-APIGW-API-KEY", assetsClientJSON.nhnPw);

var qwilightEntryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos", "Qwilight");
var twilightEntryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "IdeaProjects", "Twilight");
var taehuiEntryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "WebStormProjects", "taehui");
SetCSJavaLanguage(Path.Combine(qwilightEntryPath, "Qwilight", "Assets", "Language.json"));
SetCSLanguageSystem(Path.Combine(qwilightEntryPath, "Qwilight", "Assets", "Language.json"), Path.Combine(qwilightEntryPath, "Qwilight", "System", "LanguageSystem", "LanguageSystem.g.cs"), "Qwilight");
SetCSJavaLanguage(Path.Combine(qwilightEntryPath, "Igniter", "Resources", "Language.json"));
SetCSLanguageSystem(Path.Combine(qwilightEntryPath, "Igniter", "Resources", "Language.json"), Path.Combine(qwilightEntryPath, "Igniter", "System", "LanguageSystem", "LanguageSystem.g.cs"), "Igniter");
SetCSJavaLanguage(Path.Combine(twilightEntryPath, "src", "main", "resources", "Language.json"));
SetTSLanguage(Path.Combine(taehuiEntryPath, "qwilight-fe", "src", "assets", "language"));
SetTSLanguage(Path.Combine(taehuiEntryPath, "taehui-fe", "src", "assets", "language"));

void SetCSJavaLanguage(string languageFilePath)
{
    var textHeight = 0;
    var textStore = new SortedDictionary<string, IDictionary<string, string>>();
    var lastLanguage = string.Empty;
    var lastPropertyName = string.Empty;
    var r = new Utf8JsonReader(File.ReadAllBytes(languageFilePath));
    while (r.Read())
    {
        if (r.TokenType == JsonTokenType.StartObject)
        {
            ++textHeight;
            continue;
        }

        if (r.TokenType == JsonTokenType.EndObject)
        {
            --textHeight;
            continue;
        }

        if (textHeight == 1)
        {
            if (r.TokenType == JsonTokenType.PropertyName)
            {
                lastPropertyName = r.GetString() ?? string.Empty;
                textStore[lastPropertyName] = new SortedDictionary<string, string>();
                continue;
            }
        }

        if (textHeight == 2)
        {
            switch (r.TokenType)
            {
                case JsonTokenType.PropertyName:
                    lastLanguage = r.GetString() ?? string.Empty;
                    break;
                case JsonTokenType.String:
                    textStore[lastPropertyName][lastLanguage] = r.GetString() ?? string.Empty;
                    break;
            }
        }
    }

    using (var fs = File.OpenWrite(languageFilePath))
    using (var w = new Utf8JsonWriter(fs, new JsonWriterOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Indented = true
    }))
    {
        w.WriteStartObject();

        foreach (var language in textStore)
        {
            w.WritePropertyName(language.Key);
            w.WriteStartObject();
            foreach (var (targetLanguage, targetLanguageName) in new[] { ("ko", "ko-KR"), ("en", "en-US") })
            {
                var t = GetLanguage(targetLanguage, targetLanguageName, language.Key);
                t.Wait();
                if (!string.IsNullOrEmpty(t.Result))
                {
                    w.WritePropertyName(targetLanguageName);
                    w.WriteStringValue(t.Result);
                }
            }
            w.WriteEndObject();
        }

        w.WriteEndObject();
    }

    async Task<string> GetLanguage(string targetLanguage, string targetLanguageName, string targetPropertyName)
    {
        var defaultLanguageValue = textStore[targetPropertyName]["ko-KR"];
        if (targetLanguage == "ko")
        {
            return defaultLanguageValue;
        }

        if (textStore[targetPropertyName].TryGetValue(targetLanguageName, out var targetLanguageValue))
        {
            return targetLanguageValue;
        }
        else
        {
            Console.WriteLine($"＋{targetPropertyName}");
            return await N2Mt(targetLanguage, defaultLanguageValue).ConfigureAwait(false);
        }
    }
}

void SetTSLanguage(string languageEntryPath)
{
    var languageStore = new SortedDictionary<string, IDictionary<string, string>>();
    foreach (var targetLanguage in new[] { "ko", "en" })
    {
        languageStore[targetLanguage] = new SortedDictionary<string, string>();

        var textHeight = 0;
        var lastPropertyName = string.Empty;
        var r = new Utf8JsonReader(File.ReadAllBytes(Path.Combine(languageEntryPath, $"{targetLanguage}.json")));
        while (r.Read())
        {
            if (r.TokenType == JsonTokenType.StartObject)
            {
                ++textHeight;
                continue;
            }

            if (r.TokenType == JsonTokenType.EndObject)
            {
                --textHeight;
                continue;
            }

            if (textHeight == 1)
            {
                switch (r.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        lastPropertyName = r.GetString() ?? string.Empty;
                        break;
                    case JsonTokenType.String:
                        languageStore[targetLanguage][lastPropertyName] = r.GetString() ?? string.Empty;
                        break;
                }
            }
        }
    }

    foreach (var targetLanguage in new[] { "ko", "en" })
    {
        using (var fs = File.OpenWrite(Path.Combine(languageEntryPath, $"{targetLanguage}.json")))
        using (var w = new Utf8JsonWriter(fs, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = true
        }))
        {
            w.WriteStartObject();
            var t = GetLanguageStore(targetLanguage);
            t.Wait();
            foreach (var defaultLanguage in t.Result)
            {
                if (!string.IsNullOrEmpty(defaultLanguage.Value))
                {
                    w.WritePropertyName(defaultLanguage.Key);
                    w.WriteStringValue(defaultLanguage.Value);
                }
            }
            w.WriteEndObject();
        }
    }

    async Task<IDictionary<string, string>> GetLanguageStore(string targetLanguage)
    {
        if (targetLanguage == "ko")
        {
            return languageStore["ko"];
        }

        var targetLanguageStore = new SortedDictionary<string, string>();
        foreach (var defaultLanguage in languageStore["ko"])
        {
            if (languageStore[targetLanguage].TryGetValue(defaultLanguage.Key, out var targetLanguageValue))
            {
                targetLanguageStore[defaultLanguage.Key] = targetLanguageValue;
            }
            else
            {
                Console.WriteLine($"＋{defaultLanguage.Key}");
                targetLanguageStore[defaultLanguage.Key] = await N2Mt(targetLanguage, defaultLanguage.Value).ConfigureAwait(false);
            }
        }

        return targetLanguageStore;
    }
}

void SetCSLanguageSystem(string languageFilePath, string languageSystemFilePath, string languageSystemName)
{
    var textHeight = 0;
    var textStore = new List<string>();
    var r = new Utf8JsonReader(File.ReadAllBytes(languageFilePath));
    while (r.Read())
    {
        if (r.TokenType == JsonTokenType.StartObject)
        {
            ++textHeight;
            continue;
        }

        if (r.TokenType == JsonTokenType.EndObject)
        {
            --textHeight;
            continue;
        }

        if (textHeight == 1)
        {
            if (r.TokenType == JsonTokenType.PropertyName)
            {
                textStore.Add(r.GetString() ?? string.Empty);
                continue;
            }
        }
    }

    var builder = new StringBuilder($$"""
namespace {{languageSystemName}}
{
    public sealed partial class LanguageSystem
    {

""");

    foreach (var text in textStore)
    {
        builder.AppendLine($$"""        public string {{text}} { get; set; }""");
    }

    builder.AppendLine("""
    }
}
""");

    File.WriteAllText(languageSystemFilePath, builder.ToString());
}

async ValueTask<string> N2Mt(string targetLanguage, string src)
{
    var t = await wwwClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "https://naveropenapi.apigw.ntruss.com/nmt/v1/translation")
    {
        Content = new FormUrlEncodedContent(new[] { KeyValuePair.Create("source", "ko"), KeyValuePair.Create("target", targetLanguage), KeyValuePair.Create("text", src) }),
        Version = HttpVersion.Version20
    }).ConfigureAwait(false);
    var text = await t.Content.ReadAsStringAsync().ConfigureAwait(false);
    if (t.IsSuccessStatusCode)
    {
        return JsonSerializer.Deserialize<JSON.N2MT>(text, defaultJSONConfigure).message.result.translatedText;
    }
    else
    {
        await Console.Error.WriteLineAsync(t.StatusCode.ToString()).ConfigureAwait(false);
        return string.Empty;
    }
}