# Minimal Notepad AI

A **minimal Windows notepad-style editor with AI generation capabilities** built using:

- **C#**
- **WPF**
- **.NET 8**

The app allows you to **select text, send it to an AI model, and replace it with generated content**.  
It supports both **cloud and local AI providers**.

---

# Open Source

- License: [MIT](LICENSE)
- Contributing guide: [CONTRIBUTING.md](CONTRIBUTING.md)
- Code of conduct: [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)
- Security policy: [SECURITY.md](SECURITY.md)

---

# Features

- Simple **Notepad-style editor**
- **AI text generation**
- **Highlight → Generate → Replace**
- **Streaming responses**
- Multiple **AI providers**
- **Configurable models**
- **Local AI support**
- Keyboard shortcuts
- Lightweight and fast

---

# Supported AI Providers

The app uses an **OpenAI-compatible API format**, which allows support for multiple providers.

| Provider | Default Base URL | Example Model |
|--------|--------|--------|
| OpenAI | <https://api.openai.com/v1> | gpt-4o-mini |
| OpenRouter | <https://openrouter.ai/api/v1> | openai/gpt-4o-mini |
| Ollama | <http://localhost:11434/v1> | llama3 |
| LM Studio | <http://localhost:1234/v1> | local-model |

For **local providers (Ollama / LM Studio)** the API key can be left blank.

---

# Project Structure

```text
Minimal_Notepad_AI/
├── Config/
│   └── AppSettings.cs              — BaseUrl, ApiKey, Model, ProviderType
├── Services/
│   └── SettingsService.cs          — Load/Save JSON to %AppData%\AI-Notepad\
├── Providers/
│   ├── IAIProvider.cs              — GenerateAsync + StreamAsync interface
│   ├── OpenAICompatibleProvider.cs — Full SSE streaming impl (base for all)
│   ├── OpenAIProvider.cs           — defaults: api.openai.com, gpt-4o-mini
│   ├── OpenRouterProvider.cs       — defaults: openrouter.ai
│   ├── OllamaProvider.cs           — defaults: localhost:11434/v1
│   └── LMStudioProvider.cs         — defaults: localhost:1234/v1
├── ViewModels/
│   ├── ViewModelBase.cs            — INotifyPropertyChanged + SetProperty
│   ├── RelayCommand.cs             — ICommand wrapper
│   ├── MainViewModel.cs            — IsGenerating, StatusText, StatsText, BuildPrompt, CreateProvider
│   └── SettingsViewModel.cs        — Provider list, Save()
├── Views/
│   ├── SettingsWindow.xaml         — Provider/URL/Key/Model form
│   └── SettingsWindow.xaml.cs
├── MainWindow.xaml                 — Menu + instruction toolbar + editor + status bar
├── MainWindow.xaml.cs              — Generate logic, streaming, file I/O
└── App.xaml / App.xaml.cs
```


---

# How to Build & Run

## Using Visual Studio

1. Open Minimal_Notepad_AI.slnx

2. Use **Visual Studio 2022 or newer**

3. Press F5 to run the application.

---

## Using .NET CLI

From the project directory:

```bash
dotnet build
dotnet run
```

---

# Usage

## Typing Text

Simply type into the editor like a normal notepad.

## AI Generate

1. Select text in the editor.
2. Optionally enter an instruction.
3. Click `⚡ Generate AI` or press `Ctrl + Enter`.

The selected text will be replaced with the AI-generated output.

## Cancel Generation

While streaming, click `✕ Cancel` to stop the generation request.

## Configure AI

Open `Menu → Settings → AI Settings`.

You can configure:

- Provider
- Base URL
- API Key
- Model

Settings are saved to `%AppData%\AI-Notepad\`.

---

# Example Workflows

## Rewrite Text

- Instruction: `Rewrite professionally`
- Selected text: `This app kinda works but needs improvement.`
- Result: `This application functions but would benefit from further refinement and improvements.`

## Expand Text

- Instruction: `Expand this paragraph`

## Summarize

- Instruction: `Summarize this`

---

# Keyboard Shortcuts

| Shortcut | Action |
| --- | --- |
| `Ctrl + Enter` | Generate AI |
| `Ctrl + N` | New file |
| `Ctrl + O` | Open file |
| `Ctrl + S` | Save |
| `Ctrl + Shift + S` | Save As |

---

# Configuration Location

User settings are stored at `%AppData%\AI-Notepad\`.

Example file: `settings.json`

---

# Requirements

- Windows 10 / 11
- .NET 8 Runtime
- Visual Studio 2022 (for development)

---

# Future Improvements

Possible improvements:

- Token usage display
- Prompt templates
- AI chat mode
- Drag & drop files
- Markdown support
- Multi-tab editor
- Model auto-detection
- Plugin system
