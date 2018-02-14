namespace UI_TEST
{
    public class PopulationContext
    {
        public PersonContext Person1 { get; } = new PersonContext();
        public PersonContext Person2 { get; } = new PersonContext();

        public PopulationContext()
        {
            Person1.BestFriend = Person2;
            Person2.BestFriend = Person1;
        }
    }
}