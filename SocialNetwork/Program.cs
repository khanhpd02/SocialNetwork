using CloudinaryDotNet;
using firstapi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SocialNetwork.Entity;
using SocialNetwork.Mail;
using SocialNetwork.Middlewares;
using SocialNetwork.Repository;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;
using SocialNetwork.Service.Implement;
using SocialNetwork.Socket;
using System.Text;
using System.Text.Json.Serialization;
using WebApi.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddControllers(config =>
{
    config.Filters.Add(new ResponseFilter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement

                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
    option.EnableAnnotations();
});
// Add services to the container.
// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;

    //services.AddDbContext<DataContext>();
    services.AddDbContext<SocialNetworkContext>();
    services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
    /*services.AddCors(options => options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // Thay thế bằng tên miền của ứng dụng React của bạn
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials().AllowAnyOrigin();
    }));*/
    services.AddControllers()
        .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    services.Configure<IISServerOptions>(options =>
    {
        options.AllowSynchronousIO = true;
    });

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IPostService, PostService>();
    services.AddScoped<IEmailService, EmailService>();
    services.AddScoped<ILikeService, LikeService>();
    services.AddScoped<ICommentService, CommentService>();


    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IUserRoleRepository, UserRoleRepository>();
    services.AddScoped<IPinCodeRepository, PinCodeRepository>();
    services.AddScoped<IPostRepository, PostRepository>();
    services.AddScoped<IImageRepository, ImageRepository>();
    services.AddScoped<IVideoRepository, VideoRepository>();
    services.AddScoped<ILikeRepository, LikeRepository>();
    services.AddScoped<ICommentRepository,CommentRepository>();


    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    // Configure Cloudinary
    Account account = new Account(
  "dchu3wh6l",
  "382577841913334",
  "XjbmTfNbI1k4CTI6vVrm4rL6Nyc");

    Cloudinary cloudinary = new Cloudinary(account);
    builder.Services.AddSingleton(cloudinary);

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

var app = builder.Build();

if (true)//app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseForbiddenResponse();
app.UseUnauthorizedResponse();
app.UseRouting();
app.MapHub<ChatHub>("/chathub");
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
