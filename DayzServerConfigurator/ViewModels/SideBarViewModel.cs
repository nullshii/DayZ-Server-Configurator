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

    [Reactive] public string? Version { get; set; } = "1.0.0";
    [Reactive] public ReactiveCommand<Unit, Unit> OpenGithubCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> OpenGitlabCommand { get; set; }

    private void OpenGithub()
    {
    }
    
    private void OpenGitlab()
    {
    }
}