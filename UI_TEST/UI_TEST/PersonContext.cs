using System.ComponentModel;

namespace UI_TEST
{
    class PersonContext : INotifyPropertyChanged
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Greeting { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
