using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FFEC;
using FFECAvalonia.ViewModels;

namespace FFECAvalonia.Views;

public partial class VariablesPageView : UserControl
{
    public VariablesPageView()
    {
        InitializeComponent();
    }

    public static void TextBoxChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is null || (sender as TextBox).Text is null) return;
        string text = (sender as TextBox).Text;
        if (text.Length == 0) text = "0";
        if (int.TryParse(string.Join("", text.Where(c => char.IsDigit(c) || (c == '-' && text.Count(x => x == '-') == 1 && text.First() == '-'))), out int value)) (sender as TextBox).Text = value.ToString();
        else (sender as TextBox).Text = "0";

        JsonStorage.Configurations[Config.CurrentConfig]["Variables"][((sender as TextBox).DataContext as VariableItemTemplate).Name] = (sender as TextBox).Text;
        JsonStreamer.WriteConfigurations(JsonStorage.Configurations);
    }
}