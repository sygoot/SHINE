using CommunityToolkit.Mvvm.Input;
using HealthApp.Models;

namespace HealthApp.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}