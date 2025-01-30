using CommunityToolkit.Mvvm.ComponentModel;

namespace HealthApp.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        [ObservableProperty]
        private bool _isNextDayEnabled = false;

        public MainPageModel()
        {
            UpdateNextDayButtonState();
        }

        public void ChangeDate(int days)
        {
            SelectedDate = SelectedDate.AddDays(days);
            UpdateNextDayButtonState();
        }

        private void UpdateNextDayButtonState()
        {
            IsNextDayEnabled = SelectedDate < DateTime.Today;
        }
    }
}