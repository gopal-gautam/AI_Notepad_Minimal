using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Minimal_Notepad_AI.ViewModels;
using Minimal_Notepad_AI.Views;

namespace Minimal_Notepad_AI;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;
    private CancellationTokenSource? _cts;
    private string? _currentFilePath;
    private bool _isDirty;

    public MainWindow()
    {
        InitializeComponent();
        _vm = (MainViewModel)DataContext;
    }

    // ─── KEYBOARD SHORTCUTS ───────────────────────────────────────────────────

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        var ctrl = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0;

        if (ctrl && e.Key == Key.Enter && _vm.IsIdle)
        {
            e.Handled = true;
            _ = RunGenerateAsync();
        }
        else if (ctrl && e.Key == Key.N)
        {
            e.Handled = true;
            OnNew(this, new RoutedEventArgs());
        }
        else if (ctrl && e.Key == Key.S)
        {
            e.Handled = true;
            OnSave(this, new RoutedEventArgs());
        }
        else if (ctrl && e.Key == Key.O)
        {
            e.Handled = true;
            OnOpen(this, new RoutedEventArgs());
        }

        base.OnPreviewKeyDown(e);
    }

    // ─── AI GENERATION ────────────────────────────────────────────────────────

    private async void OnGenerate(object sender, RoutedEventArgs e)
        => await RunGenerateAsync();

    private async Task RunGenerateAsync()
    {
        var selectedText = Editor.SelectedText;

        if (string.IsNullOrWhiteSpace(selectedText))
        {
            MessageBox.Show(
                "Please select some text in the editor first.\n\n" +
                "The selected text becomes the AI prompt.",
                "No Text Selected",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var selStart = Editor.SelectionStart;
        var selLen   = Editor.SelectionLength;
        var before   = Editor.Text[..selStart];
        var after    = Editor.Text[(selStart + selLen)..];
        var prompt   = _vm.BuildPrompt(selectedText);

        _cts             = new CancellationTokenSource();
        _vm.IsGenerating = true;
        _vm.StatusText   = "Generating…";

        try
        {
            var provider    = _vm.CreateProvider();
            var accumulated = new StringBuilder();

            await foreach (var token in provider.StreamAsync(prompt, _cts.Token))
            {
                accumulated.Append(token);
                Editor.Text       = before + accumulated + after;
                Editor.CaretIndex = selStart + accumulated.Length;
            }

            _isDirty       = true;
            _vm.StatusText = "Done";
        }
        catch (OperationCanceledException)
        {
            // Restore original selection on cancel
            Editor.Text            = before + selectedText + after;
            Editor.SelectionStart  = selStart;
            Editor.SelectionLength = selLen;
            _vm.StatusText         = "Cancelled";
        }
        catch (Exception ex)
        {
            // Restore original selection on error
            Editor.Text            = before + selectedText + after;
            Editor.SelectionStart  = selStart;
            Editor.SelectionLength = selLen;
            _vm.StatusText         = "Error";
            MessageBox.Show(
                $"AI generation failed:\n\n{ex.Message}",
                "Generation Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            _vm.IsGenerating = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void OnCancelGeneration(object sender, RoutedEventArgs e)
        => _cts?.Cancel();

    // ─── EDITOR EVENTS ────────────────────────────────────────────────────────

    private void OnEditorTextChanged(object sender, TextChangedEventArgs e)
    {
        _isDirty = true;
        _vm.UpdateStats(Editor.Text);
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        var name = _currentFilePath is null ? "Untitled" : Path.GetFileName(_currentFilePath);
        Title = _isDirty ? $"*{name} – AI Notepad" : $"{name} – AI Notepad";
    }

    // ─── MENU: FILE ───────────────────────────────────────────────────────────

    private void OnNew(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDiscard()) return;
        Editor.Clear();
        _currentFilePath = null;
        _isDirty         = false;
        Title            = "AI Notepad";
        _vm.StatusText   = "Ready";
    }

    private void OnOpen(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDiscard()) return;

        var dlg = new OpenFileDialog
        {
            Filter     = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            DefaultExt = ".txt"
        };

        if (dlg.ShowDialog() != true) return;

        try
        {
            Editor.Text      = File.ReadAllText(dlg.FileName);
            _currentFilePath = dlg.FileName;
            _isDirty         = false;
            Title            = $"{Path.GetFileName(dlg.FileName)} – AI Notepad";
            _vm.StatusText   = $"Opened: {dlg.FileName}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open file:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        if (_currentFilePath is null) SaveAs();
        else WriteFile(_currentFilePath);
    }

    private void OnSaveAs(object sender, RoutedEventArgs e) => SaveAs();

    private void SaveAs()
    {
        var dlg = new SaveFileDialog
        {
            Filter     = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            DefaultExt = ".txt",
            FileName   = _currentFilePath is null
                ? "Untitled.txt"
                : Path.GetFileName(_currentFilePath)
        };

        if (dlg.ShowDialog() == true) WriteFile(dlg.FileName);
    }

    private void WriteFile(string path)
    {
        try
        {
            File.WriteAllText(path, Editor.Text);
            _currentFilePath = path;
            _isDirty         = false;
            Title            = $"{Path.GetFileName(path)} – AI Notepad";
            _vm.StatusText   = $"Saved: {path}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not save file:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnExit(object sender, RoutedEventArgs e)
    {
        if (ConfirmDiscard()) Application.Current.Shutdown();
    }

    private bool ConfirmDiscard()
    {
        if (!_isDirty || string.IsNullOrEmpty(Editor.Text)) return true;
        return MessageBox.Show(
            "You have unsaved changes. Discard them?",
            "AI Notepad",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    // ─── MENU: SETTINGS ───────────────────────────────────────────────────────

    private void OnOpenSettings(object sender, RoutedEventArgs e)
    {
        var win = new SettingsWindow { Owner = this };
        win.ShowDialog();
    }
}
