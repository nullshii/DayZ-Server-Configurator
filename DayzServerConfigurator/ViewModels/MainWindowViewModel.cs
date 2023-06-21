using System;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace DayzServerConfigurator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        SelectBasePathCommand = ReactiveCommand.Create(SelectBasePath);
        CheckModListCommand = ReactiveCommand.Create(CheckModList);
        ConfigureServerCommand = ReactiveCommand.Create(ConfigureServer);

        SideBar = new SideBarViewModel();

        KeysPathHolder = new PathHolderViewModel { Label = "Keys path:" };
        AddonsPathHolder = new PathHolderViewModel { Label = "Addons path:" };
        ProfilePathHolder = new PathHolderViewModel { Label = "Profile path:" };
        BattlEyePathHolder = new PathHolderViewModel { Label = "BattlEye path:" };
    }

    public ReactiveCommand<Unit, Unit> SelectBasePathCommand { get; set; }
    public ReactiveCommand<Unit, Unit> CheckModListCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ConfigureServerCommand { get; set; }

    [Reactive] public SideBarViewModel SideBar { get; set; }

    [Reactive] public PathHolderViewModel KeysPathHolder { get; set; }
    [Reactive] public PathHolderViewModel AddonsPathHolder { get; set; }
    [Reactive] public PathHolderViewModel ProfilePathHolder { get; set; }
    [Reactive] public PathHolderViewModel BattlEyePathHolder { get; set; }

    private void ConfigureServer()
    {
        throw new NotImplementedException();
    }

    private void CheckModList()
    {
        throw new NotImplementedException();
    }

    private void SelectBasePath()
    {
        throw new NotImplementedException();
    }
}