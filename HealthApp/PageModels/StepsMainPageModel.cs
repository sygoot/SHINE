using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HealthApp.Models;

namespace HealthApp.PageModels
{
    public partial class StepsMainPageModel : ObservableObject
    {
        private readonly HealthService _healthService;

        public ObservableCollection<Tip> Tips { get; }

        [ObservableProperty]
        private int _steps;
        public StepsMainPageModel(HealthService healthService)
        {
            _healthService = healthService;
            Tips = new ObservableCollection<Tip>();
        }

        private void OnAppearing()
        {
            Tips.Clear();

            // Dodaj przykładowe tipy
            Tips.Add(new Tip { Title = "Tip 1: Get enough sleep every night", IsChecked = false });
            Tips.Add(new Tip { Title = "Tip 2: Maintain a regular bedtime schedule", IsChecked = true });
            Tips.Add(new Tip { Title = "Tip 3: Avoid caffeine before bedtime", IsChecked = false });
        }
    }
}
