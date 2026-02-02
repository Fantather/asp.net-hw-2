namespace asp.net_hw_2.UserManager
{
    public class UserManager
    {
        private readonly List<User> _users = new();
        public IReadOnlyList<User> GetAll() => _users.AsReadOnly();
        public User? GetById(Guid id) => _users.FirstOrDefault(u => u.Id == id);
        public User? GetByName(string name) => _users.FirstOrDefault(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        public void Create(User user) => _users.Add(user);
        public bool Update(Guid id, User updatedUser)
        {
            var existingUser = GetById(id);
            if (existingUser is null)
            {
                return false;
            }
            existingUser.UpdateFrom(updatedUser);
            return true;
        }
        public bool Delete(Guid id)
        {
            var user = GetById(id);
            if (user is null)
            {
                return false;
            }
            _users.Remove(user);
            return true;
        }
    }
}
