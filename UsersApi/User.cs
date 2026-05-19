namespace UsersApi
{
    public class User(Guid id, string name, string lastName)
    {
        public Guid Id { get; set; } = id;

        public string Name { get; set; } = name;

        public string LastName { get; set; } = lastName;
    }
}
