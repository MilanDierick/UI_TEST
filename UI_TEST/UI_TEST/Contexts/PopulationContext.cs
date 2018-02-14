using System.Linq;
using ReactiveUI;

namespace UI_TEST
{
    public class PopulationContext : ReactiveObject
    {
        private PersonContext _selection;

        public PopulationContext()
        {
            People = new ReactiveList<PersonContext>
            {
                new PersonContext(),
                new PersonContext(),
                new PersonContext(),
                new PersonContext(),
                new PersonContext()
            };

            Selection = People.FirstOrDefault();
        }

        public ReactiveList<PersonContext> People { get; }

        public PersonContext Selection
        {
            get => _selection;
            set => this.RaiseAndSetIfChanged(ref _selection, value);
        }
    }
}