using System.Windows;
using Minimal_Notepad_AI.ViewModels;

namespace Minimal_Notepad_AI.Views;

public partial class SettingsWindow : Window
{
    private readonly SettingsViewModel _vm;

    public SettingsWindow()
    {
        _vm = new SettingsViewModel();
        DataContext = _vm;
        InitializeComponent();
        // Populate PasswordBox from loaded settings (binding not supported natively)
        ApiKeyBox.Password = _vm.ApiKey;
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        _vm.ApiKey = ApiKeyBox.Password;
        _vm.Save();
        DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
