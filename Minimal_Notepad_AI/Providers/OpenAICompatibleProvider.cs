using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Minimal_Notepad_AI.Providers;

/// <summary>
/// Base provider that works with any OpenAI-compatible API
/// (OpenAI, OpenRouter, Ollama /v1, LM Studio, etc.)
/// </summary>
public class OpenAICompatibleProvider : IAIProvider
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly string _model;

    public OpenAICompatibleProvider(string baseUrl, string apiKey, string model)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _model = model;
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
        if (!string.IsNullOrWhiteSpace(apiKey))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsync(
            $"{_baseUrl}/chat/completions",
            BuildContent(prompt, stream: false),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(cancellationToken));

        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;
    }

    public async IAsyncEnumerable<string> StreamAsync(
        string prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/chat/completions")
        {
            Content = BuildContent(prompt, stream: true)
        };

        using var response = await _http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data: "))
                continue;

            var data = line[6..]; // skip "data: "
            if (data == "[DONE]") break;

            string? token = null;
            try
            {
                using var doc = JsonDocument.Parse(data);
                var choices = doc.RootElement.GetProperty("choices");
                if (choices.GetArrayLength() > 0)
                {
                    var delta = choices[0].GetProperty("delta");
                    if (delta.TryGetProperty("content", out var content))
                        token = content.GetString();
                }
            }
            catch { /* skip malformed SSE chunks */ }

            if (!string.IsNullOrEmpty(token))
                yield return token;
        }
    }

    private StringContent BuildContent(string prompt, bool stream)
    {
        var payload = new
        {
            model = _model,
            messages = new[] { new { role = "user", content = prompt } },
            stream
        };
        return new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
    }
}
