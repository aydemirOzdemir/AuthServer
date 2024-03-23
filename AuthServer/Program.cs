using AuthServer.Core.Configuration;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitofWork;
using AuthServer.Data.Context;
using AuthServer.Data.Repositories;
using AuthServer.Data.UnitofWork;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Configuration;
using SharedLibrary.Extensions;
using SharedLibrary.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOptions"));
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.AddScoped(typeof(IGenericService<,>),typeof(GenericService<,>));
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
builder.Services.AddDbContext<AuthServerDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
});
builder.Services.AddIdentity<UserApp, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;

}).AddEntityFrameworkStores<AuthServerDbContext>().AddDefaultTokenProviders();
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOption>();// burada jwt içinde doðrulama iþlemleri hangi parametrelere göre yapýlacaðýný kontrolünü saðlamak için options patternde yazdýðýmýz sýnýfý alýp iþlemleri yapýyoruz.
builder.Services.AddAuthenticationService(tokenOptions);








var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
