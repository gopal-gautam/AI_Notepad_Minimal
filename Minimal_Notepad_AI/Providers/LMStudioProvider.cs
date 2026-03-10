namespace Minimal_Notepad_AI.Providers;

public class LMStudioProvider(string baseUrl, string apiKey, string model)
    : OpenAICompatibleProvider(
        string.IsNullOrWhiteSpace(baseUrl) ? "http://localhost:1234/v1" : baseUrl,
        apiKey,
        string.IsNullOrWhiteSpace(model) ? "local-model" : model);
