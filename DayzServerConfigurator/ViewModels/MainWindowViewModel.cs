using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace DayzServerConfigurator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly OpenFolderDialog _folderDialog;

    public MainWindowViewModel()
    {
        SelectBasePathCommand = ReactiveCommand.Create(SelectBasePath);
        UpdateModListCommand = ReactiveCommand.Create(UpdateModList);
        ConfigureServerCommand = ReactiveCommand.Create(ConfigureServer);

        ModDirectories = new ObservableCollection<DirectoryInfo>();

        SideBar = new SideBarViewModel();

        KeysPathHolder = new PathHolderViewModel { Label = "Keys path:" };
        AddonsPathHolder = new PathHolderViewModel { Label = "Addons path:" };
        ProfilePathHolder = new PathHolderViewModel { Label = "Profile path:" };
        BattlEyePathHolder = new PathHolderViewModel { Label = "BattlEye path:" };

        _folderDialog = new OpenFolderDialog { Title = "Select DayZ server directory" };
    }

    private DirectoryInfo? ServerDirectory { get; set; }

    public ReactiveCommand<Unit, Unit> SelectBasePathCommand { get; set; }
    public ReactiveCommand<Unit, Unit> UpdateModListCommand { get; set; }
    public ReactiveCommand<Unit, Task> ConfigureServerCommand { get; set; }

    [Reactive] public ObservableCollection<DirectoryInfo> ModDirectories { get; set; }

    [Reactive] public SideBarViewModel SideBar { get; set; }

    [Reactive] public PathHolderViewModel KeysPathHolder { get; set; }
    [Reactive] public PathHolderViewModel AddonsPathHolder { get; set; }
    [Reactive] public PathHolderViewModel ProfilePathHolder { get; set; }
    [Reactive] public PathHolderViewModel BattlEyePathHolder { get; set; }
    
    [Reactive] public float Progress { get; set; }

    private async Task ConfigureServer()
    {
        if (ServerDirectory == null)
        {
            // TODO: show messagebox to select server directory
            return;
        }

        await RemoveOldModFiles();
        await CopyNewModFiles();
        await GenerateBatFile();
    }

    private void UpdateModList()
    {
        if (ServerDirectory == null)
        {
            // TODO: show messagebox to select server directory
            return;
        }

        ModDirectories = new ObservableCollection<DirectoryInfo>(ServerDirectory.GetDirectories("@*",
            SearchOption.TopDirectoryOnly));
    }

    private async void SelectBasePath()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime app) return;
        string? selectedDirectory = await _folderDialog.ShowAsync(app.MainWindow);
        if (selectedDirectory == null) return;

        ServerDirectory = new DirectoryInfo(selectedDirectory);

        KeysPathHolder.Path =
            ServerDirectory.GetDirectories().First(info => info.Name.ToLower() == "keys").FullName;

        AddonsPathHolder.Path =
            ServerDirectory.GetDirectories().First(info => info.Name.ToLower() == "addons").FullName;

        BattlEyePathHolder.Path =
            ServerDirectory.GetDirectories().First(info => info.Name.ToLower() == "battleye").FullName;

        ProfilePathHolder.Path =
            (ServerDirectory.GetDirectories().FirstOrDefault(info => info.Name.ToLower() == "profile")
             ?? Directory.CreateDirectory(Path.Combine(ServerDirectory.FullName, "profile"))).FullName;
    }
    
    private async Task RemoveOldModFiles()
    {
        string[] baseAddons = await File.ReadAllLinesAsync(Path.Combine("settings", "DefaultAddonList.txt"));
        string[] baseKeys = await File.ReadAllLinesAsync(Path.Combine("settings", "DefaultKeyList.txt"));

        List<FileInfo> modKeys = new DirectoryInfo(KeysPathHolder.Path).GetFiles()
            .Where(info => baseKeys.Contains(info.Name) == false).ToList();

        List<FileInfo> modAddons = new DirectoryInfo(AddonsPathHolder.Path).GetFiles()
            .Where(info => baseAddons.Contains(info.Name) == false).ToList();

        await Task.Factory.StartNew(() =>
        {
            for (var i = 0; i < modAddons.Count; i++)
            {
                Progress = (float) i / modAddons.Count;
                FileInfo file = modAddons[i];
                file.Delete();
            }

            for (var i = 0; i < modKeys.Count; i++)
            {
                Progress = (float) i / modKeys.Count;
                FileInfo file = modKeys[i];
                file.Delete();
            }
        });
    }
    
    private async Task CopyNewModFiles()
    {
        foreach (DirectoryInfo modDirectory in ModDirectories)
        {
            DirectoryInfo addons = modDirectory.GetDirectories().First(info => info.Name.ToLower() == "addons");
            DirectoryInfo keys = modDirectory.GetDirectories().First(info => info.Name.ToLower() == "keys");

            await Task.Factory.StartNew(() =>
            {
                FileInfo[] files = addons.GetFiles();
                for (var i = 0; i < files.Length; i++)
                {
                    Progress = (float) i / files.Length;
                    FileInfo addon = files[i];
                    addon.CopyTo(Path.Combine(AddonsPathHolder.Path, addon.Name), true);
                }

                FileInfo[] infos = keys.GetFiles();
                for (var i = 0; i < infos.Length; i++)
                {
                    Progress = (float) i / files.Length;
                    FileInfo key = infos[i];
                    key.CopyTo(Path.Combine(KeysPathHolder.Path, key.Name), true);
                }
            });
        }
    }
    
    private async Task GenerateBatFile()
    {
        if (ServerDirectory == null) return;

        string mods = ModDirectories
            .Select(directoryInfo => directoryInfo.Name)
            .Aggregate((first, second) => $"{first};{second}");

        mods = string.IsNullOrWhiteSpace(mods) ? "" : $"\"-mod={mods}\"";

        string fileContent = "DayZServer_x64.exe " +
                             "-config=serverDZ.cfg " +
                             "-cpuCount=2 " +
                             "-dologs -adminlog -netlog -freezecheck " +
                             $"\"-BEpath={BattlEyePathHolder.Path}\" " +
                             $"\"-profiles={ProfilePathHolder.Path}\" " +
                             mods;

        await File.WriteAllTextAsync(Path.Combine(ServerDirectory.FullName, "Start.bat"), fileContent);
    }
}