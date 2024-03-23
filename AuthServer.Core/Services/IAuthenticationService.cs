using AuthServer.Core.Dtos;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IAuthenticationService
    {//Burada kimlik doğrulama işlemlerinin yapıldığı servisler.

        Task<ResponseDto<TokenDto>> CreateTokenAsync(LoginDto loginDto);
        Task<ResponseDto<ClientTokenDto>> CreateTokenByClientAsync(ClientLoginDto clientLoginDto);
        Task<ResponseDto<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken);
        Task<ResponseDto<NoDataDto>> RevokeRefreshToken(string refreshToken);
    }
}
