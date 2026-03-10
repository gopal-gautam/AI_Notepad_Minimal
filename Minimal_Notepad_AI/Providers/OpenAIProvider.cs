namespace Minimal_Notepad_AI.Providers;

public class OpenAIProvider(string baseUrl, string apiKey, string model)
    : OpenAICompatibleProvider(
        string.IsNullOrWhiteSpace(baseUrl) ? "https://api.openai.com/v1" : baseUrl,
        apiKey,
        string.IsNullOrWhiteSpace(model) ? "gpt-4o-mini" : model);
