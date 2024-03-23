using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitofWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> clients;
        private readonly ITokenService tokenService;
        private readonly UserManager<UserApp> userManager;// kullanıcının olup olmadığını kontrol edebilmek için.
        private readonly IUnitofWork unitofWork;
        private readonly IGenericRepository<UserRefreshToken> repository;

        public AuthenticationService(IOptions<List<Client>> clients,ITokenService tokenService,UserManager<UserApp> userManager,IUnitofWork unitofWork,IGenericRepository<UserRefreshToken> repository)
        {
            this.clients = clients.Value;
            this.tokenService = tokenService;
            this.userManager = userManager;
            this.unitofWork = unitofWork;
            this.repository = repository;
        }
        public async Task<ResponseDto<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
           if(loginDto==null) throw new ArgumentNullException(nameof(loginDto));
           var user=await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return ResponseDto<TokenDto>.Fail("Email veya Password Yanlış",400,true);
            if(!await userManager.CheckPasswordAsync(user,loginDto.Password))
                return ResponseDto<TokenDto>.Fail("Email veya Password Yanlış", 400, true);
            var token = tokenService.CreateToken(user);
            var userRefreshToken= await repository.Where(x=>x.UserId==user.Id).SingleOrDefaultAsync();

            if(userRefreshToken==null)
                await repository.Add(new UserRefreshToken { Expiration=token.RefreshTokenExpiration,UserId=user.Id,Code=token.RefreshToken});
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
                 repository.Update(userRefreshToken);
            }
            await unitofWork.CommitAsync();
            return ResponseDto<TokenDto>.Success(token,200);
        }

        public async Task<ResponseDto<ClientTokenDto>> CreateTokenByClientAsync(ClientLoginDto clientLoginDto)
        {
            var client = clients.SingleOrDefault(x=>x.Id==clientLoginDto.ClientId&&x.Secret==clientLoginDto.ClientSecret);
            if (client == null)
                return ResponseDto<ClientTokenDto>.Fail("ClientId veya ClientSecret bulunamadı",404,true);

            var token = tokenService.CreateTokenByClient(client);

            return ResponseDto<ClientTokenDto>.Success(token, 200);

        }

        public async Task<ResponseDto<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            var existrefresToken = await repository.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existrefresToken == null)
                return ResponseDto<TokenDto>.Fail("RefreshToken bulunamadı", 404, true);
            var user = await userManager.FindByIdAsync(existrefresToken.UserId);
            if (user == null)
                return ResponseDto<TokenDto>.Fail("Kullanıcı bulunamadı", 404, true);
            var token=tokenService.CreateToken(user);
            existrefresToken.Code = token.RefreshToken;
            existrefresToken.Expiration = token.RefreshTokenExpiration;
             repository.Update(existrefresToken);
            await unitofWork.CommitAsync();
            return ResponseDto<TokenDto>.Success(token, 200);

        }

        public async Task<ResponseDto<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existrefresToken = await repository.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existrefresToken == null)
                return ResponseDto<NoDataDto>.Fail("RefreshToken bulunamadı", 404, true);
            repository.Remove(existrefresToken);
            await unitofWork.CommitAsync();
            return ResponseDto<NoDataDto>.Success(200);
        }
    }
}
