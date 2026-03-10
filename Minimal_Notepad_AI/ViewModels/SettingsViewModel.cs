using System.Collections.Generic;
using Minimal_Notepad_AI.Config;
using Minimal_Notepad_AI.Services;

namespace Minimal_Notepad_AI.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService = new();

    private string _selectedProvider;
    private string _baseUrl;
    private string _apiKey;
    private string _model;

    public static IReadOnlyDictionary<string, (string BaseUrl, string Model)> ProviderDefaults { get; } =
        new Dictionary<string, (string, string)>
        {
            ["OpenAI"]     = ("https://api.openai.com/v1",        "gpt-4o-mini"),
            ["OpenRouter"] = ("https://openrouter.ai/api/v1",     "openai/gpt-4o-mini"),
            ["Ollama"]     = ("http://localhost:11434/v1",         "llama3.2"),
            ["LM Studio"]  = ("http://localhost:1234/v1",         "local-model"),
            ["Custom"]     = ("",                                  ""),
        };

    public IEnumerable<string> ProviderNames => ProviderDefaults.Keys;

    public string SelectedProvider
    {
        get => _selectedProvider;
        set
        {
            if (SetProperty(ref _selectedProvider, value) &&
                ProviderDefaults.TryGetValue(value, out var defaults))
            {
                BaseUrl = defaults.BaseUrl;
                Model   = defaults.Model;
            }
        }
    }

    public string BaseUrl
    {
        get => _baseUrl;
        set => SetProperty(ref _baseUrl, value);
    }

    public string ApiKey
    {
        get => _apiKey;
        set => SetProperty(ref _apiKey, value);
    }

    public string Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

    public SettingsViewModel()
    {
        var s = _settingsService.Load();
        _selectedProvider = s.ProviderType;
        _baseUrl          = s.BaseUrl;
        _apiKey           = s.ApiKey;
        _model            = s.Model;
    }

    public void Save()
    {
        _settingsService.Save(new AppSettings
        {
            ProviderType = SelectedProvider,
            BaseUrl      = BaseUrl,
            ApiKey       = ApiKey,
            Model        = Model,
        });
    }
}
