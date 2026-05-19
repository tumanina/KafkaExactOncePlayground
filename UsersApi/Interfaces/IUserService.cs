namespace UsersApi.Interfaces
{
    public interface IUserService
    {
        public Task<User> CreateUser(User user);
    }
}
