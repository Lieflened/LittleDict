using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyDict;

public class TranslationService
{
    // 使用 MyMemory
    private static readonly HttpClient _http = new()
    {
        Timeout = TimeSpan.FromSeconds(6)
    };

    public async Task<string> TranslateAsync(string text, string sourceLang = "en", string targetLang = "zh-CN")
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        if(System.Text.Encoding.UTF8.GetByteCount(text) > 500)
        {
            return "文本过长！";
        }

        var url =   $"https://api.mymemory.translated.net/get" +
                    $"?q={Uri.EscapeDataString(text)}" +
                    $"&langpair={sourceLang}|{targetLang}";
        
        try
        {
            var json = await _http.GetStringAsync(url).ConfigureAwait(false);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if(root.TryGetProperty("responseStatus", out var status) && status.GetInt32() != 200)
            {
                return "翻译服务暂不可用！";
            }
            if(root.TryGetProperty("responseData", out var data) &&
                data.TryGetProperty("translatedText", out var translated))
            {
                var result = translated.GetString();
                return string.IsNullOrWhiteSpace(result) ? "未找到译文" : result;
            }
        }
        catch (TaskCanceledException)
        {
            return "翻译请求超时";
        }
        catch (Exception)
        {
            return "翻译失败，可能是网络问题";
        }
        
        return "未获取到译文";
    }
}