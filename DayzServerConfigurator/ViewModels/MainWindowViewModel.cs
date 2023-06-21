using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace DayzServerConfigurator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly OpenFolderDialog _folderDialog;
    private ServerConfigurator? _configurator;

    public MainWindowViewModel()
    {
        SelectBasePathCommand = ReactiveCommand.Create(SelectBasePath);
        UpdateModListCommand = ReactiveCommand.Create(UpdateModList);
        ConfigureServerCommand = ReactiveCommand.Create(ConfigureServer);

        ModList = new ObservableCollection<string>();
        
        SideBar = new SideBarViewModel();

        KeysPathHolder = new PathHolderViewModel { Label = "Keys path:" };
        AddonsPathHolder = new PathHolderViewModel { Label = "Addons path:" };
        ProfilePathHolder = new PathHolderViewModel { Label = "Profile path:" };
        BattlEyePathHolder = new PathHolderViewModel { Label = "BattlEye path:" };

        _folderDialog = new OpenFolderDialog { Title = "Select DayZ server directory" };
    }

    public ReactiveCommand<Unit, Unit> SelectBasePathCommand { get; set; }
    public ReactiveCommand<Unit, Unit> UpdateModListCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ConfigureServerCommand { get; set; }

    [Reactive] public ObservableCollection<string> ModList { get; set; }

    [Reactive] public SideBarViewModel SideBar { get; set; }

    [Reactive] public PathHolderViewModel KeysPathHolder { get; set; }
    [Reactive] public PathHolderViewModel AddonsPathHolder { get; set; }
    [Reactive] public PathHolderViewModel ProfilePathHolder { get; set; }
    [Reactive] public PathHolderViewModel BattlEyePathHolder { get; set; }

    private void ConfigureServer()
    {
        if (_configurator == null)
        {
            // TODO: show messagebox to select server directory
            return;
        }

        _configurator.Configure();
    }
    
    private void UpdateModList()
    {
        if (_configurator == null)
        {
            // TODO: show messagebox to select server directory
            return;
        }
        
        _configurator.UpdateModList();
        ModList.Clear();
        
        foreach (DirectoryInfo directory in _configurator.ModDirectories)
        {
            ModList.Add(directory.Name);
        }
    }

    private async void SelectBasePath()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime app) return;

        string? selectedDirectory = await _folderDialog.ShowAsync(app.MainWindow);

        if (selectedDirectory == null) return;

        _configurator = new ServerConfigurator(new DirectoryInfo(selectedDirectory));
        
        KeysPathHolder.Path = _configurator.KeysDirectory.FullName;
        AddonsPathHolder.Path = _configurator.AddonsDirectory.FullName;
        ProfilePathHolder.Path = _configurator.ProfileDirectory.FullName;
        BattlEyePathHolder.Path = _configurator.BattlEyeDirectory.FullName;
    }
}