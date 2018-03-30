using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ReactiveUI;

namespace UI_TEST
{
    public class PopulationContext : ReactiveObject
    {
        private PersonContext _selection;
        private PersonContext _selectedFriend;
        private ObservableAsPropertyHelper<bool> _hasSelection;

        public PopulationContext()
        {
            People = new ReactiveList<PersonContext>();

            AddFriend = ReactiveCommand.Create(
                () => People.Add(new PersonContext(this)));

            IObservable<bool> hasSelection = this
                .WhenAnyValue(x => x.Selection)
                .Select(s => s != null)
                .DistinctUntilChanged();

            _hasSelection = hasSelection
                .ToProperty(this, x => x.HasSelection);

            bool FriendRemover()
            {
                var selection = Selection;
                if (selection == null) return false;

                var wasRemoved = People.Remove(selection);
                selection.Dispose();

                Selection = null;

                return wasRemoved;
            }

            RemoveFriend = ReactiveCommand.Create(FriendRemover, hasSelection);

            Selection = People.FirstOrDefault();
        }

        public bool HasSelection => _hasSelection.Value;

        public ReactiveCommand<Unit, Unit> AddFriend { get; }

        public ReactiveCommand<Unit, bool> RemoveFriend { get; }

        public ReactiveList<PersonContext> People { get; }

        public PersonContext Selection
        {
            get => _selection;
            set => this.RaiseAndSetIfChanged(ref _selection, value);
        }

        public PersonContext SelectedFriend
        {
            get => _selectedFriend;
            set => this.RaiseAndSetIfChanged(ref _selectedFriend, value);
        }
    }
}