using Minimal_Notepad_AI.Config;
using Minimal_Notepad_AI.Providers;
using Minimal_Notepad_AI.Services;

namespace Minimal_Notepad_AI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService = new();

    private string _instructionText = string.Empty;
    private bool _isGenerating;
    private string _statusText = "Ready";
    private string _statsText = "Characters: 0 | Words: 0";

    public string InstructionText
    {
        get => _instructionText;
        set => SetProperty(ref _instructionText, value);
    }

    public bool IsGenerating
    {
        get => _isGenerating;
        set
        {
            SetProperty(ref _isGenerating, value);
            OnPropertyChanged(nameof(IsIdle));
        }
    }

    public bool IsIdle => !_isGenerating;

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public string StatsText
    {
        get => _statsText;
        set => SetProperty(ref _statsText, value);
    }

    /// <summary>Creates a provider from current saved settings.</summary>
    public IAIProvider CreateProvider()
    {
        var s = _settingsService.Load();
        return s.ProviderType switch
        {
            "OpenAI"      => new OpenAIProvider(s.BaseUrl, s.ApiKey, s.Model),
            "OpenRouter"  => new OpenRouterProvider(s.BaseUrl, s.ApiKey, s.Model),
            "Ollama"      => new OllamaProvider(s.BaseUrl, s.ApiKey, s.Model),
            "LM Studio"   => new LMStudioProvider(s.BaseUrl, s.ApiKey, s.Model),
            _             => new OpenAICompatibleProvider(s.BaseUrl, s.ApiKey, s.Model),
        };
    }

    /// <summary>Builds the final prompt sent to the AI.</summary>
    public string BuildPrompt(string selectedText)
    {
        return string.IsNullOrWhiteSpace(InstructionText)
            ? selectedText
            : $"{InstructionText.Trim()}\n\n{selectedText}";
    }

    /// <summary>Updates the character/word stats shown in the status bar.</summary>
    public void UpdateStats(string text)
    {
        var words = string.IsNullOrWhiteSpace(text)
            ? 0
            : text.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries).Length;
        StatsText = $"Characters: {text.Length:N0} | Words: {words:N0}";
    }

    public AppSettings LoadSettings() => _settingsService.Load();
}
