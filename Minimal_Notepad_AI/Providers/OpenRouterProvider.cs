namespace Minimal_Notepad_AI.Providers;

public class OpenRouterProvider(string baseUrl, string apiKey, string model)
    : OpenAICompatibleProvider(
        string.IsNullOrWhiteSpace(baseUrl) ? "https://openrouter.ai/api/v1" : baseUrl,
        apiKey,
        string.IsNullOrWhiteSpace(model) ? "openai/gpt-4o-mini" : model);
