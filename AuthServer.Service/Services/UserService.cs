using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using AuthServer.Service.Mappings;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(UserManager<UserApp> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<ResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
           var user=new UserApp { Email = createUserDto.Email, UserName=createUserDto.UserName };
            var result=await userManager.CreateAsync(user,createUserDto.Password);
            if (!result.Succeeded)
            {
                var errors=result.Errors.Select(x=>x.Description).ToList();
                return ResponseDto<UserAppDto>.Fail(new ErrorDto(errors, true), 404);
            }
            return ResponseDto<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }

        public async Task<ResponseDto<NoDataDto>> CreateUserRoles(string email)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                await roleManager.CreateAsync(new IdentityRole { Name = "Manager" });
            }
            var currentUser=await userManager.FindByEmailAsync(email);
            await userManager.AddToRoleAsync(currentUser,"Admin");
            userManager.AddToRoleAsync(currentUser,"Yönetici");


            return ResponseDto<NoDataDto>.Success(200);
        }

        public async Task<ResponseDto<UserAppDto>> GetUserByAsync(string userName)
        {
           var user= await userManager.FindByNameAsync(userName);
            if (user == null)
                return ResponseDto<UserAppDto>.Fail("Kullanıcı bulunamadı",404,true);
            return ResponseDto<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }

        
    }
}
