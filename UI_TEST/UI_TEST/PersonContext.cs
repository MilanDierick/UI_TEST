using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;
using UI_TEST.Annotations;

namespace UI_TEST
{
    internal class PersonContext : ReactiveObject
    {
        private string _firstName;
        private string _lastName;
        private string _greeting;

        public PersonContext()
        {
            this.WhenAnyValue(x => x.FirstName, x => x.LastName,
                    (firstName, lastName) => $"Hello {firstName} {lastName}")
                .BindTo(this, x => x.Greeting);
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

        public string Greeting
        {
            get => _greeting;
            private set => this.RaiseAndSetIfChanged(ref _greeting, value);
        }
    }
}
