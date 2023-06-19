using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DayzServerConfigurator.Views;

public partial class SideBar : UserControl
{
    public SideBar()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}