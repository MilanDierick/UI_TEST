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

        public PopulationContext()
        {
            People = new ReactiveList<PersonContext>();

            AddFriend = ReactiveCommand.Create(
                () => People.Add(new PersonContext(this)));

            RemoveFriend = ReactiveCommand.Create(
                () =>
                {
                    var selection = Selection;
                    if (selection == null)
                        return false;

                    var wasRemoved = People.Remove(selection);
                    selection.Dispose();

                    Selection = null;

                    return wasRemoved;
                });

            Selection = People.FirstOrDefault();
        }

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