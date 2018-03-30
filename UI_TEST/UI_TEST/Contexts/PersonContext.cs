using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace UI_TEST
{
    public class PersonContext : ReactiveObject, IDisposable
    {
        private readonly ObservableAsPropertyHelper<string> _fullName;
        private readonly ObservableAsPropertyHelper<string> _bestFriendName;
        private readonly IDisposable _itemsRemovedSubscription;

        private string _firstName = "Foo";
        private string _lastName = "Bar";
        private PersonContext _bestFriend;

        public PersonContext(PopulationContext population)
        {
            _fullName = this
                .WhenAnyValue(x => x.FirstName, x => x.LastName,
                    (firstName, lastName) => $"{firstName} {lastName}")
                .ToProperty(this, x => x.FullName);

            _bestFriendName = this
                .WhenAnyValue(x => x.BestFriend)
                .Select(bf => bf == null 
                    ? Observable.Return("I don't have a best friend") 
                    : bf.WhenAnyValue(f => f.FullName).Select(fn => $"{fn} is my best friend."))
                .Switch()
                .ToProperty(this, x => x.BestFriendName);

            _itemsRemovedSubscription = population.People.BeforeItemsRemoved
                .Do(p => Console.WriteLine($@"{FullName} => Person removed {p?.FullName}"))
                .Where(p => p == BestFriend)
                .Select(p => (PersonContext)null)
                .BindTo(this, x => x.BestFriend);
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

        public void Dispose()
        {
            _fullName?.Dispose();
            _bestFriendName?.Dispose();
            _itemsRemovedSubscription?.Dispose();
        }
    }
}
