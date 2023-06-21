using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DayzServerConfigurator.Views;

public partial class PathHolder : UserControl
{
    public PathHolder()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}