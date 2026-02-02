namespace asp.net_hw_2.UserManager
{
    public class User
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; set; }
        public string City { get; set; }
        public int Age { get; set; }

        // Для преобразования из JSON
        public User(string name, string city, int age, Guid? id = null)
        {
            Name = name;
            City = city;
            Age = age;

            if (id is not null)
            {
                Id = id.Value;
            }
        }

        public void UpdateFrom(User source)
        {
            Name = source.Name;
            City = source.City;
            Age = source.Age;
        }
    }
}
