using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HealthApp.Models;

namespace HealthApp.PageModels
{
    class SleepMainPageModel : ObservableObject
    {
        public Command AppearingCommand => new Command(() =>
        {
            Tips = new ObservableCollection<Tip>
    {
        new Tip { Title = "Tip 1: Placeholder text for the tip.", IsChecked = false },
        new Tip { Title = "Tip 2: Placeholder text for the tip.", IsChecked = true },
        new Tip { Title = "Tip 3: Placeholder text for the tip.", IsChecked = false }
    };
        });

        public ObservableCollection<Tip> Tips { get; private set; }
    }
}
