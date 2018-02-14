using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace UI_TEST
{
    public class PersonContext : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string> _fullName;
        private readonly ObservableAsPropertyHelper<string> _bestFriendName;

        private string _firstName = "Foo";
        private string _lastName = "Bar";
        private PersonContext _bestFriend;

        public PersonContext()
        {
            _fullName = this
                .WhenAnyValue(x => x.FirstName, x => x.LastName,
                    (firstName, lastName) => $"{firstName} {lastName}")
                .ToProperty(this, x => x.FullName);

            _bestFriendName = this
                .WhenAnyValue(x => x.BestFriend.FullName)
                .Select(fullName => $"{fullName} is my best friend.")
                .ToProperty(this, x => x.BestFriendName);
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

        public PersonContext BestFriend
        {
            get => _bestFriend;
            set => this.RaiseAndSetIfChanged(ref _bestFriend, value);
        }

        public string FullName => _fullName.Value;

        public string BestFriendName => _bestFriendName.Value;

    }
}
