namespace Minimal_Notepad_AI.Providers;

public interface IAIProvider
{
    Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> StreamAsync(string prompt, CancellationToken cancellationToken = default);
}
