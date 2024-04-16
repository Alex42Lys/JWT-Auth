using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication4;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();

using(ApplicationContext db = new ApplicationContext())
{
    //db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    //LoginData lg1 = new LoginData("tom@gmail.com", "12345");
    ///LoginData lg2 = new LoginData("bob@gmail.com", "55555");
   // db.LoginDataArr.AddRange(lg1,lg2);
    db.SaveChanges();
}


var people = new List<LoginData>();
using (ApplicationContext db = new ApplicationContext())
{
    people = db.LoginDataArr.ToList();

    /*Console.WriteLine("Users list:");
    foreach (LoginData u in people)
    {
        Console.WriteLine($"{u.Login} - {u.Password}");
    }*/
}
/*  var people = new List<LoginData>
{
  new LoginData("tom@gmail.com", "12345"),
  new LoginData("bob@gmail.com", "55555")
};*/

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });    



var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapPost("/login", (LoginData loginData) =>
{
    // находим пользователя 
    LoginData? person = people.FirstOrDefault(p => p.Login == loginData.Login && p.Password == loginData.Password);
    // если пользователь не найден, отправляем статусный код 401
    if (person is null) return Results.Unauthorized();

    var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Login) };
    // создаем JWT-токен
    var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

    // формируем ответ
    var response = new
    {
        access_token = encodedJwt,
        username = person.Login
    };

    return Results.Json(response);
});



app.Map("/data", [Authorize] () => new { message = "Data acquired." });




app.Run();



public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    const string KEY = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}



//record class Person(string Email, string Password);