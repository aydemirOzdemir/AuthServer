using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configuration;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomTokenAuth
    {public static IServiceCollection AddAuthenticationService(this IServiceCollection serviceCollection,CustomTokenOption options)
        {
       

            serviceCollection.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //İki ayrı üyelik sistemi olarabir bayiler için ayrı üyelik olabilir. Kullanıcılar için farklı bir login olabilir. Burada farklı iki şema ekleyebilirdik  bizde tek var onun için sabit bir JwtBearerDefaults kullanıyorum.
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
            {
               
                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {// burada jwt içindeki verilerin birbirinii nasıl doğrulayacağını belirttiğimiz bir yapı kuruyoruz. Bu karsı tarafın istek yaptığında benim projeme onuun verileri
                    ValidIssuer = options.Issuer,// hangi Issurerdan veri geleceğini belirttik.
                    ValidAudience = options.Audience[0],
                    // burada hangi proje istek yapacak onu belirttik.
                    ValidateIssuerSigningKey = true,// imzayı doğrulasın mı?
                    ValidateAudience = true, // audience doğrula 
                    ValidateLifetime = true,// Ömrünün geçerli olup olmadığını kontrol et.
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(options.SecurityKey),// gelen imzayı neye göre doğrulayacağını belirtiyoruz.
                    ClockSkew = TimeSpan.Zero
                    // farklı serverlarda farklı zaman dilimlerinde istek için ömür belirlenebilir onun için identity kütüp. default değerinin 1 saat vermisse bize 1 saat 5 dk ömürle çıkar bunu hataları minimize etmek için  yapar biz onu minimize ediyoruz.
                };




            });


            return serviceCollection;
        }
    }
}
