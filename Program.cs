using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using slate;
using slate.AuthenticationAndAuthorization;
using slate.BlackboardsServices;
using slate.DbServices;
using slate.DrawingServices;
using slate.UsersServices;

const long maxMessageBufferSize = 524288;

var builder = WebApplication.CreateBuilder(); //(new WebApplicationOptions { WebRootPath = "wwwroot/dist" });

const string connection = "Host=localhost;Port=5432;Database=slate;Username=postgres;Password=Kvaskovu20031986";
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));

builder.Services.AddAuthorization();

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = AuthOptions.Audience,
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
                if (
                    !string.IsNullOrEmpty(accessToken)
                    && path.StartsWithSegments("/imageExchanging")
                )
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

builder.Services.AddCors(options =>
    options.AddPolicy(
        "CorsPolicy",
        policyBuilder =>
        {
            policyBuilder
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed((_) => true);
        }
    )
);
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

//app.MapHub<DrawingHub>("/drawing");
app.MapHub<ImageHub>(
    "/imageExchanging",
    options =>
    {
        ArgumentNullException.ThrowIfNull(options);

        options.ApplicationMaxBufferSize = maxMessageBufferSize;
        //options.TransportMaxBufferSize = maxMessageBufferSize;
    }
);

app.MapPost(
    "/registration",
    async (User regData, ApplicationContext db) =>
    {
        ArgumentNullException.ThrowIfNull(regData);
        ArgumentNullException.ThrowIfNull(db);

        await db.Users.AddAsync(regData);
        await db.SaveChangesAsync();

        return regData;
    }
);

app.MapPost(
    "/login",
    async (User loginData, ApplicationContext db) =>
    {
        ArgumentNullException.ThrowIfNull(loginData);
        ArgumentNullException.ThrowIfNull(db);

        var user = await db.Users.FirstOrDefaultAsync(p =>
            p.Email == loginData.Email && p.Password == loginData.Password
        );

        if (user is null)
            return Results.Unauthorized();

        if (user is not { Email: not null, Name: not null }) return null;
        var claims = new List<Claim> { new(ClaimTypes.Email, user.Email), new(ClaimTypes.Name, user.Name) };

        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(
                AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256
            )
        );
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var response = new
        {
            id = user.Id,
            userName = user.Name,
            userEmail = user.Email,
            accessToken = encodedJwt,
        };

        return Results.Json(response);

    }
);

app.MapPost(
    "/newBlackboard",
    //[Authorize]
    async (NewBlackboardData blackboardData, ApplicationContext db) =>
    {
        ArgumentNullException.ThrowIfNull(blackboardData);
        ArgumentNullException.ThrowIfNull(db);

        var newBlackboard = new Blackboard()
        {
            Name = blackboardData.Name,
            Description = blackboardData.Description
        };

        if (!int.TryParse(blackboardData.OwnerId, out int ownerId))
            return Results.Json(
                new { isSuccess = false, failureReason = "OwnerId isn't integer.", }
            );

        var owner = db.Users.Find(ownerId);

        if (owner is null)
            return Results.Json(new { isSuccess = false, failureReason = "Owner doesn't exist.", });

        newBlackboard.Users.Add(owner);

        await db.Blackboards.AddAsync(newBlackboard);

        await db.SaveChangesAsync();

        return Results.Json(new { isSuccess = true, failureReason = "", });
    }
);

app.MapDelete(
    "deleteBlackboard/{idStr}",
    //[Authorize]
    async (int id, ApplicationContext db) => //Разобраться с idStr и id
    {
        ArgumentNullException.ThrowIfNull(db);

        var deletedBlackboard = db.Blackboards.Find(id);

        if (deletedBlackboard is null)
            return Results.Json(
                new { isSuccess = false, failureReason = "Deleted blackboard doesn't exist.", }
            );

        db.Blackboards.Remove(deletedBlackboard);

        await db.SaveChangesAsync();

        return Results.Json(new { isSuccess = true, failureReason = "", });
    }
);

app.MapGet(
    "userBlackboards/{userId}",
    //[Authorize]
    (string userId, ApplicationContext db) =>
    {
        ArgumentNullException.ThrowIfNull(db);

        if (!int.TryParse(userId, out int id))
        {
            return Results.Json(
                new { isSuccess = false, failureReason = "User id isn't integer.", }
            );
        }

        var users = db.Users.Include(u => u.Blackboards).ToList();

        var user = users.Find(u => u.Id == id);

        if (user is null)
            return Results.Json(new { isSuccess = false, failureReason = "User doesn't exist.", });

        var json = Results.Json(user.Blackboards);

        return json;
    }
);

// app.MapGet(
//     "blackboardData/{blackboardId}",
//     (string blackboardId) =>
//     {
//         if (!int.TryParse(blackboardId, out int id))
//             return Results.BadRequest($"Id = {blackboardId} is not an integer!");

//         var response = testDb[id];

//         return Results.Json(response);
//     }
// );

app.Run();
