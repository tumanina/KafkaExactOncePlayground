using Microsoft.AspNetCore.Mvc;
using UsersApi.Interfaces;

namespace UsersApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost(Name = "CreateUser")]
        public async Task<User> Create([FromBody] User user)
        {
            return await _userService.CreateUser(user);
        }
    }
}
