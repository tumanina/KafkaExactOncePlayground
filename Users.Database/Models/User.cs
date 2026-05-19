namespace Users.Database.Models;

public class User
{
    public User()
    { }

    public User(string name, string lastName)
    {
        Id = Guid.NewGuid();
        Name = name;
        LastName = lastName;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
