using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;

    var jwtConfig = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtConfig["SecretKey"]!;
    options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = jwtConfig["Issuer"],
    ValidAudience = jwtConfig["Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
};
});
builder.Services.AddSingleton<ISessionStore, InMemoryStore>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IBrowserService, BrowserService>();
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TimeResponse>();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapSessionEndpoints();
app.MapAuthEndpoints();
app.MapPlayEndpoints();


app.Run();






