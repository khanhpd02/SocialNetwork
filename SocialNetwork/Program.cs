﻿using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SocialNetwork.Entity;
using SocialNetwork.Mail;
using SocialNetwork.Middlewares;
using SocialNetwork.Repository;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Socket;
//using SocialNetwork.Socket;
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

    services.AddDistributedMemoryCache(); // Register an in-memory cache implementation for IDistributedCache
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // Configure your session options
    });

    services.AddHttpContextAccessor();

    //services.AddDbContext<DataContext>();
    services.AddDbContext<SocialNetworkContext>();
    //services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
    services.AddCors(options => options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // Thay thế bằng tên miền của ứng dụng React của bạn
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    }));
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

    var serviceTypes = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(s => s.GetTypes())
    .Where(p => p.Namespace != null && p.Namespace.Contains("Service") && p.Name.EndsWith("Service") && p.GetInterfaces().Any())
    .ToList();
    foreach (var serviceType in serviceTypes)
    {
        services.AddScoped(serviceType.GetInterfaces().First(), serviceType);
    }
    //services.AddScoped<IUserService, UserService>();
    //services.AddScoped<IPostService, PostService>();
    //services.AddScoped<IEmailService, EmailService>();
    //services.AddScoped<ILikeService, LikeService>();
    //services.AddScoped<ICommentService, CommentService>();
    //services.AddScoped<IInforService, InforService>();


    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IUserRoleRepository, UserRoleRepository>();
    services.AddScoped<IPinCodeRepository, PinCodeRepository>();
    services.AddScoped<IPostRepository, PostRepository>();
    services.AddScoped<IImageRepository, ImageRepository>();
    services.AddScoped<IVideoRepository, VideoRepository>();
    services.AddScoped<ILikeRepository, LikeRepository>();
    services.AddScoped<ICommentRepository, CommentRepository>();
    services.AddScoped<IInforRepository, InforRepository>();
    services.AddScoped<IGroupChatRepository, GroupChatRepository>();
    services.AddScoped<IUserGroupChatRepository, UserGroupChatRepository>();
    services.AddScoped<INotifyRepository, NotifyRepository>();
    services.AddScoped<IMasterDataRepository, MasterDataRepository>();
    services.AddScoped<IFriendRepository, FriendRepository>();



    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


    // Configure Cloudinary
    Account account = new Account(
  "khanhpd",
  "694226254617467",
  "LRTR_fpACSFMKTH9il0cXC_rgvo");

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
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chathub");
    endpoints.MapHub<CommentHub>("/commentHub");
    endpoints.MapHub<VideoHub>("/videoHub");
});
app.Run();
