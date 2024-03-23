using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            return ActionResultInstance(await userService.CreateUserAsync(createUserDto));
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return ActionResultInstance(await userService.GetUserByAsync(User.Identity.Name));
        }

    }
}
