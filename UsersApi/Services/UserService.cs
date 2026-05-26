using Newtonsoft.Json;
using Users.Database;
using Users.Database.Models;
using UsersApi.Interfaces;
using DbUser = Users.Database.Models.User;
using DbEvent = Users.Database.Models.Event;

namespace UsersApi.Services
{
    public class UserService(ILogger<UserService> logger, UsersContext dbContext) : IUserService
    {
        private readonly UsersContext _dbContext = dbContext;
        private readonly ILogger<UserService> _logger = logger;

        public async Task<User> CreateUser(User user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }
            var dbUser = new DbUser { Id = user.Id, Name = user.Name, LastName = user.LastName };

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _dbContext.Users.Add(dbUser);
                await _dbContext.SaveChangesAsync();

                var userEvent = new DbEvent
                { 
                    Id = Guid.NewGuid(), 
                    CorrelationId = Guid.NewGuid(),
                    EventType = "user_created",
                    Version = 1,
                    Payload = JsonConvert.SerializeObject(user)
                };
                _dbContext.Events.Add(userEvent);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                //todo: log exception message with correlationId on middleware
                throw;
            }

            return user;
        }
    }
}
