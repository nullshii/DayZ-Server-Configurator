using ReactiveUI.Fody.Helpers;

namespace DayzServerConfigurator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        SideBar = new SideBarViewModel();
    }

    [Reactive] public SideBarViewModel SideBar { get; set; }
}