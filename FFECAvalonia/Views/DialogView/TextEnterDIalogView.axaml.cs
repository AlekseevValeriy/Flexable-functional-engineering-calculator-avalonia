using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;

namespace FFECAvalonia.Views.DialogView;

public partial class TextEnterDIalogView : Window
{
    public TextEnterDIalogView()
    {
        InitializeComponent();
        OkButton.AddHandler(Button.ClickEvent, OkButtonClick);
        CanceButton.AddHandler(Button.ClickEvent, CanceButtonClick);
    }

    public string? Result { get; private set; }


    public void OkButtonClick(object? sender, RoutedEventArgs e)
    {
        Close(InputTextBox.Text);
    }
    private void CanceButtonClick(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}