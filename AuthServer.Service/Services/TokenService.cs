using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configuration;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> userManager;
        private readonly CustomTokenOption tokenOption;

        public TokenService(UserManager<UserApp> userManager,IOptions<CustomTokenOption> options)
        {
            this.userManager = userManager;
            this.tokenOption = options.Value;
        }

        private string CreateRefreshToken()
        {
            var numberByte=new byte[32];
            using var rnd=RandomNumberGenerator.Create();
            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
           // return Guid.NewGuid().ToString();
        }

        private async Task<IEnumerable<Claim>> GetClaims(UserApp user,List<string> audiences)
        { 
            var userRoles=await userManager.GetRolesAsync(user);
            //Userın rollerini de claim olarak eklemek için user managerla rolleri çekiyoruz.
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())

            };

            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));//Burada hangi serverlara istek yapıp yapamayacağımızı kontrol ettiğimiz yerdeyiz.Her token oluştuğunda audience alanı açar ve içerisine tanımlı audienceleri yükler.
            userList.AddRange(userRoles.Select(x=>new Claim(ClaimTypes.Role,x)));// userRolesde gelen veriler stirng değerler bunların hepsini select ile dönüp değerlerini bir claim tipi olarak ekliyoruz rolleri.


            return userList;
        }



        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims= new List<Claim>();

            claims.AddRange(client.Audience.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));// her bir string için yeni bir audience oluştursun.
            claims.Add(
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));//tokena özel id
            claims.Add(
         new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString())); //Clienttın idsine özel bir claim (sub burada öznel anlamında)
            return claims;
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(tokenOption.AccessTokenExpiration);//appsettingle eşlediğimiz modelimizden gelen dakika değerini gelip tokenın accesstoken değerine ekleyecek. ve süreyi belirleyecek.
            var refreshTokenExpiration = DateTime.Now.AddMinutes(tokenOption.RefreshTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(tokenOption.SecurityKey);//security key değerini yine appsettingden alacak ve bir keye çevirecek.

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);// burada artık bir imza oluşturuyoruz. security keyi hashledik.

            // şimdi tokenı oluşturuyoruz.
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
           (issuer: tokenOption.Issuer,
          expires: accessTokenExpiration,
          notBefore: DateTime.Now , // daha önceki saatler için geçerli olmasın diyoruz. Burada accesstoken expire ile suan arasında geçerli
          claims: GetClaims(userApp,tokenOption.Audience).Result,
          signingCredentials:signingCredentials
           );
            var handler=new JwtSecurityTokenHandler();//Tokenı oluşturan taraf bu handler
            var token=handler.WriteToken(jwtSecurityToken); // artık token oluşmuş oldu 
            var tokenDto = new TokenDto
            {
                AccessToken= token,
                RefreshToken=CreateRefreshToken(),
                AccessTokenExpiration=accessTokenExpiration,
                RefreshTokenExpiration=refreshTokenExpiration
            };

            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(tokenOption.AccessTokenExpiration);//appsettingle eşlediğimiz modelimizden gelen dakika değerini gelip tokenın accesstoken değerine ekleyecek. ve süreyi belirleyecek.
           
            var securityKey = SignService.GetSymmetricSecurityKey(tokenOption.SecurityKey);//security key değerini yine appsettingden alacak ve bir keye çevirecek.

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);// burada artık bir imza oluşturuyoruz. security keyi hashledik.

            // şimdi tokenı oluşturuyoruz.
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
           (issuer: tokenOption.Issuer,
          expires: accessTokenExpiration,
          notBefore: DateTime.Now, // daha önceki saatler için geçerli olmasın diyoruz. Burada accesstoken expire ile suan arasında geçerli
          claims: GetClaimsByClient(client),
          signingCredentials: signingCredentials
           );
            var handler = new JwtSecurityTokenHandler();//Tokenı oluşturan taraf bu handler
            var token = handler.WriteToken(jwtSecurityToken); // artık token oluşmuş oldu 
            var tokenDto = new ClientTokenDto
            {
                AccessToken = token,
              
                AccessTokenExpiration = accessTokenExpiration,
                
            };

            return tokenDto;





        }
    }
}
