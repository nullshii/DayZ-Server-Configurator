using System.Diagnostics;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace DayzServerConfigurator.ViewModels;

public class SideBarViewModel : ViewModelBase
{
    public SideBarViewModel()
    {
        OpenGithubCommand = ReactiveCommand.Create(OpenGithub);
        OpenGitlabCommand = ReactiveCommand.Create(OpenGitlab);
    }

    [Reactive] public string? Version { get; set; } = "1.0.1";
    [Reactive] public ReactiveCommand<Unit, Unit> OpenGithubCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> OpenGitlabCommand { get; set; }

    private void OpenGithub()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/nullshii/DayZ-Server-Configurator",
            UseShellExecute = true
        });
    }

    private void OpenGitlab()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://gitlab.com/nullshii/dayz-server-configurator",
            UseShellExecute = true
        });
    }
}