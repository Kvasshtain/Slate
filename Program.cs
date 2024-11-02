using SignalRApp;
using DrawingServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using slate.UsersServices;
using AuthenticationAndAuthorization;

// var render = new ImageRender();
// render.TestDraw();

// условная бд с пользователями
var people = new List<User> //ВЫНЕСИ В ОТДЕЛЬНЫЙ ФАЙЛ И СДЕЛАЙ В ВИДЕ ПОЛНОЦЕННОЙ БД
 {
    new(1, "tom@gmail.com", "12345", "Вася Пупкин"),
    new(2, "bob@gmail.com", "55555", "Иванов Петя")
};

long maxMessageBufferSize = 524288;

var builder = WebApplication.CreateBuilder();//(new WebApplicationOptions { WebRootPath = "wwwroot/dist" });

// string connection = "Host=localhost;Port=5432;Database=blackboardObjectsdb;Username=postgres;Password=Kvaskovu20031986";
// builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/imageExchanging"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
}); 

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.MaximumReceiveMessageSize = maxMessageBufferSize;
});

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials()
                   .SetIsOriginAllowed((host) => true);
        }));
builder.Services.AddSingleton<IBlackboardStoreService, BlackboardStoreService>();
//builder.Services.AddScoped<IPointStoreService, PointStoreService>();
//builder.Services.AddScoped<IRenderingService, RenderingService>();

var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

//app.UseCors(builder => builder.AllowAnyOrigin());

// app.MapGet("/Star", async () => 
// {
//     string path = "test.jpg";
//     byte[] fileContent = await File.ReadAllBytesAsync(path);  // считываем файл в массив байтов
//     string contentType = "image/jpg";       // установка mime-типа
//     string downloadName = "test.jpg";  // установка загружаемого имени
//     return Results.File(fileContent, contentType, downloadName);
// });

//app.MapHub<DrawingHub>("/drawing");
app.MapHub<ImageHub>("/imageExchanging", options =>
{
    options.ApplicationMaxBufferSize = maxMessageBufferSize;
    //options.TransportMaxBufferSize = maxMessageBufferSize;
});

app.MapPost("/login", (User loginData) => 
{
    // находим пользователя 
    User? user = people.FirstOrDefault(p => p.Email == loginData.Email && p.Password == loginData.Password);
    // если пользователь не найден, отправляем статусный код 401
    if(user is null) return Results.Unauthorized();
     
    var claims = new List<Claim> {new Claim(ClaimTypes.Name, user.Email) };
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
        id = user.Id,
        userName = user.Name,
        userEmail = user.Email,
        accessToken = encodedJwt,
    };
 
    return Results.Json(response);
});

app.Run();