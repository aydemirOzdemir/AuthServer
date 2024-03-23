using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    [Route("api/[controller]/[action]")]//Direk benim verdiğim isme göre bu actionlara erişebilsin ondan action ekledim.
    [ApiController]
    public class AuthController : CustomBaseController
    {
        private readonly IAuthenticationService authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            var result=await authenticationService.CreateTokenAsync(loginDto);
            return ActionResultInstance(result);

        }

        [HttpPost]

        public async Task<IActionResult> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var result=await authenticationService.CreateTokenByClientAsync(clientLoginDto);
            return ActionResultInstance(result);
        }

        [HttpPost]

        public async Task< IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await authenticationService.RevokeRefreshToken(refreshTokenDto.RefreshToken);
            return ActionResultInstance(result);
        }

        [HttpPost]

        public async Task<IActionResult> CreateRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await authenticationService.CreateTokenByRefreshTokenAsync(refreshTokenDto.RefreshToken);
            return ActionResultInstance(result);
        }

    }
}
