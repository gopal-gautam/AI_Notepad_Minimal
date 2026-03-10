namespace Minimal_Notepad_AI.Config;

public class AppSettings
{
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o-mini";
    public string ProviderType { get; set; } = "OpenAI";
}
