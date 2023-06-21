using ReactiveUI.Fody.Helpers;

namespace DayzServerConfigurator.ViewModels;

public class PathHolderViewModel : ViewModelBase
{
    public PathHolderViewModel()
    {
        Label = "Label";
        Path = "";
    }

    [Reactive] public string Label { get; set; }
    [Reactive] public string Path { get; set; }
}