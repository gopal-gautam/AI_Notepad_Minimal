namespace Minimal_Notepad_AI.Providers;

public class OllamaProvider(string baseUrl, string apiKey, string model)
    : OpenAICompatibleProvider(
        string.IsNullOrWhiteSpace(baseUrl) ? "http://localhost:11434/v1" : baseUrl,
        apiKey,
        string.IsNullOrWhiteSpace(model) ? "llama3.2" : model);
