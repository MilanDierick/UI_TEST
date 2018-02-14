using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;
using UI_TEST.Annotations;

namespace UI_TEST
{
    public class PersonContext : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string> _greeting;

        private string _firstName;
        private string _lastName;

        public PersonContext()
        {
            _greeting = this
                .WhenAnyValue(x => x.FirstName, x => x.LastName,
                    (firstName, lastName) => $"Hello {firstName} {lastName}")
                .ToProperty(this, x => x.Greeting);
        }

        public string FirstName
        {
            get => _firstName;
            set => this.RaiseAndSetIfChanged(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }

        public string Greeting => _greeting.Value;
    }
}
