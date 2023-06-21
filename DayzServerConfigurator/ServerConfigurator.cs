using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

    private DirectoryInfo ServerDirectory { get; }

    public DirectoryInfo AddonsDirectory { get; }
    public DirectoryInfo KeysDirectory { get; }
    public DirectoryInfo BattlEyeDirectory { get; }
    public DirectoryInfo ProfileDirectory { get; }
    public List<DirectoryInfo> ModDirectories { get; private set; }

    public void Configure()
    {
        RemoveOldModFiles();
        CopyNewModFiles();
        // TODO: Generate Start.bat file
    }

    public void UpdateModList()
    {
        ModDirectories = ServerDirectory.GetDirectories("@*", SearchOption.TopDirectoryOnly).ToList();
    }

    private async void CopyNewModFiles()
    {
        foreach (DirectoryInfo modDirectory in ModDirectories)
        {
            DirectoryInfo addons = modDirectory.GetDirectories().First(info => info.Name.ToLower() == "addons");
            DirectoryInfo keys = modDirectory.GetDirectories().First(info => info.Name.ToLower() == "keys");

            await Task.Factory.StartNew(() =>
            {
                foreach (FileInfo addon in addons.GetFiles())
                {
                    addon.CopyTo(Path.Combine(AddonsDirectory.FullName, addon.Name), true);
                }

                foreach (FileInfo key in keys.GetFiles())
                {
                    key.CopyTo(Path.Combine(KeysDirectory.FullName, key.Name), true);
                }
            });
        }
    }

    private async void RemoveOldModFiles()
    {
        string[] baseAddons = await File.ReadAllLinesAsync(Path.Combine("settings", "DefaultAddonList.txt"));
        string[] baseKeys = await File.ReadAllLinesAsync(Path.Combine("settings", "DefaultKeyList.txt"));

        IEnumerable<FileInfo> modKeys = KeysDirectory.GetFiles()
            .Where(info => baseKeys.Contains(info.Name) == false);

        IEnumerable<FileInfo> modAddons = AddonsDirectory.GetFiles()
            .Where(info => baseAddons.Contains(info.Name) == false);

        await Task.Factory.StartNew(() =>
        {
            foreach (FileInfo file in modAddons)
            {
                file.Delete();
            }

            foreach (FileInfo file in modKeys)
            {
                file.Delete();
            }
        });
    }
}