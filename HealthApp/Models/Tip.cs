using System.Collections.ObjectModel;

namespace HealthApp.Models
{
    public class Tip
    {
        public string Title { get; set; }
        public bool IsChecked { get; set; }

        public ObservableCollection<Tip> Tips { get; set; }
    }
}