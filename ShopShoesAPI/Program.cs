using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShopShoesAPI.admin;
using ShopShoesAPI.auth;
using ShopShoesAPI.common;
using ShopShoesAPI.Data;
using ShopShoesAPI.email;
using ShopShoesAPI.model;
using ShopShoesAPI.user;
using ShopShoesAPI.product;
using System.Text;

using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using ShopShoesAPI.cart;
using ShopShoesAPI.order;
using System.Reflection;
using Payment.Application.Services.Merchant;
using Payment.Application.Services.PaymentDestination;
using Payment.Application.Services.PaymentNotification;
using Payment.Application.Services.PaymentSignature;
using Payment.Application.Services.PaymentTransaction;
using ShopShoesAPI.CheckoutServices.Payment;
using Payment.Application.Services.Payment;

var builder = WebApplication.CreateBuilder(args);

// Initialize the Configuration variable
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// Add services to the container.

builder.Services.AddControllers();

// Entity Framework
builder.Services.AddDbContext<MyDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("MyDb"));
});

// Identity
builder.Services.AddIdentity<UserEnityIndetity, IdentityRole>()
    .AddEntityFrameworkStores<MyDbContext>().AddDefaultTokenProviders();

// Add config for require Email
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

// Add Email Config
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

// Scoped
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<MyDbContext>();
builder.Services.AddScoped<IAuth, AuthService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IEmail, EmailService>();
builder.Services.AddScoped<IAdmin, AdminService>();
builder.Services.AddScoped<ICart, CartService>();
builder.Services.AddScoped<IProduct, ProductService>();
builder.Services.AddScoped<IOrder, OrderService>();

builder.Services.AddScoped<IMerchant, MerchantService>();
builder.Services.AddScoped<IPaymentDes, PaymentDesService>();
builder.Services.AddScoped<IPaymentNoti, PaymentNotiService>();
builder.Services.AddScoped<IPaymentSig, PaymentSigService>();
builder.Services.AddScoped<IPaymentTrans, PaymentTransService>();
builder.Services.AddScoped<IPayment, PaymentService>();



builder.Services.AddHttpContextAccessor();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// redis
var redisUri = builder.Configuration["AppSettings:RedisURI"];
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisUri)
);


// Authentication
var secretKey = builder.Configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,

        ValidIssuer = builder.Configuration["AppSettings:ValidIssuer"],
        ValidAudience = builder.Configuration["AppSettings:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes)
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Shop Shoes APIS",
        Version = "v1"
    });

    // Define the Bearer token security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Require Bearer token for all operations
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";  
    var path = Path.Combine(AppContext.BaseDirectory, xmlFileName);
    c.IncludeXmlComments(path);
});

// Configure session state
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.Run();
