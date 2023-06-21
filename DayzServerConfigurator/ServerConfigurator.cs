using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DayzServerConfigurator;

public class ServerConfigurator
{
    public ServerConfigurator(DirectoryInfo serverDirectory)
    {
        ServerDirectory = serverDirectory;
        
        AddonsDirectory = serverDirectory.GetDirectories().First(info => info.Name.ToLower() == "addons");
        KeysDirectory = serverDirectory.GetDirectories().First(info => info.Name.ToLower() == "keys");
        BattlEyeDirectory = serverDirectory.GetDirectories().First(info => info.Name.ToLower() == "battleye");
        
        ProfileDirectory = serverDirectory.GetDirectories().FirstOrDefault(info => info.Name.ToLower() == "profile") 
                           ?? Directory.CreateDirectory(Path.Combine(serverDirectory.FullName, "profile"));

        ModDirectories = new List<DirectoryInfo>();
    }

    public DirectoryInfo ServerDirectory { get; }
    public DirectoryInfo AddonsDirectory { get; }
    public DirectoryInfo KeysDirectory { get; }
    public DirectoryInfo BattlEyeDirectory { get; }
    public DirectoryInfo ProfileDirectory { get; }
    public List<DirectoryInfo> ModDirectories { get; private set; }

    public void Configure()
    {
        RemoveOldModFiles();
        // TODO: Copy new mod files
        // TODO: Generate Start.bat file
    }

    private async void RemoveOldModFiles()
    {
        string[] baseAddons = await File.ReadAllLinesAsync(Path.Combine("settings", "DefaultAddonList.txt"));

        IEnumerable<FileInfo> modAddons = AddonsDirectory.GetFiles()
            .Where(info => baseAddons.Contains(info.Name) == false);

        foreach (FileInfo file in modAddons)
        {
            file.Delete();
        }

        string[] baseKeys = await File.ReadAllLinesAsync(Path.Combine("settings", "DefaultKeyList.txt"));

        IEnumerable<FileInfo> modKeys = KeysDirectory.GetFiles()
            .Where(info => baseKeys.Contains(info.Name) == false);

        foreach (FileInfo file in modKeys)
        {
            file.Delete();
        }
    }

    public void UpdateModList()
    {
        ModDirectories = ServerDirectory.GetDirectories("@*", SearchOption.TopDirectoryOnly).ToList();
    }
}