using firstapi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Entity;
using SocialNetwork.Mail;
using SocialNetwork.Service;
using SocialNetwork.Service.Implement;
using System.Text;
using System.Text.Json.Serialization;
using WebApi.Helpers;





var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;

    //services.AddDbContext<DataContext>();
    services.AddDbContext<SocialNetworkContext>();
    services.AddCors();
    services.AddControllers()
        .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IEmailService, EmailService>();

}

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//Authorization
var secretKey = builder.Configuration["AppSettings:Secret"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
        ClockSkew = TimeSpan.Zero,
    };
});




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseForbiddenResponse();
app.UseUnauthorizedResponse();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
