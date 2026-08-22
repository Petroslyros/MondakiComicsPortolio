using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MondakiComics.Configuration;
using MondakiComics.Core.Helpers;
using MondakiComics.Data;
using MondakiComics.Exceptions;
using MondakiComics.Repositories;
using MondakiComics.Services;
using MondakiComics.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Get all environment variables
var dbHost = Environment.GetEnvironmentVariable("MONDAKI_DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("MONDAKI_DB_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("MONDAKI_DB_NAME") ?? "mondakicomics";
var dbUser = Environment.GetEnvironmentVariable("MONDAKI_DB_USER") ?? "mondaki_user";
var dbPass = Environment.GetEnvironmentVariable("MONDAKI_DB_PASS") ?? "";
var jwtSecret = Environment.GetEnvironmentVariable("MONDAKI_JWT_SECRET") ?? "dev-placeholder-secret";
var r2AccessKey = Environment.GetEnvironmentVariable("MONDAKI_R2_ACCESS_KEY") ?? "";
var r2SecretKey = Environment.GetEnvironmentVariable("MONDAKI_R2_SECRET_KEY") ?? "";
var r2Endpoint = Environment.GetEnvironmentVariable("MONDAKI_R2_ENDPOINT") ?? "";
var r2Bucket = Environment.GetEnvironmentVariable("MONDAKI_R2_BUCKET") ?? "mondaki-comics";
var r2PublicUrl = Environment.GetEnvironmentVariable("MONDAKI_R2_PUBLIC_URL") ?? "";

builder.Configuration["R2:AccessKey"] = r2AccessKey;
builder.Configuration["R2:SecretKey"] = r2SecretKey;
builder.Configuration["R2:Endpoint"] = r2Endpoint;
builder.Configuration["R2:Bucket"] = r2Bucket;
builder.Configuration["R2:PublicUrl"] = r2PublicUrl;
builder.Configuration["Authentication:SecretKey"] = jwtSecret;

// Build connection string from environment variables
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
connString = connString!
    .Replace("{MONDAKI_DB_HOST}", dbHost)
    .Replace("{MONDAKI_DB_PORT}", dbPort)
    .Replace("{MONDAKI_DB_NAME}", dbName)
    .Replace("{MONDAKI_DB_USER}", dbUser)
    .Replace("{MONDAKI_DB_PASS}", dbPass);

// Register DbContext
builder.Services.AddDbContext<MondakiDbContext>(options =>
    options.UseNpgsql(connString));

// Add UnitOfWork DI
builder.Services.AddRepositories();

builder.Services.AddScoped<IImageUploadService, ImageUploadService>();

// Add ApplicationService DI
builder.Services.AddScoped<IApplicationService, ApplicationService>();



// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());

// Serilog
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

// JWT Authentication
var issuer = Environment.GetEnvironmentVariable("MONDAKI_JWT_ISSUER") ?? "https://localhost:5002";
builder.Configuration["Authentication:Issuer"] = issuer;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = issuer,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        b => b.WithOrigins("https://mondaki-comics-front.vercel.app", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Controllers + Newtonsoft
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MondakiComics API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// ---- Everything above this line is registration (builder) ----
var app = builder.Build();
// ---- Everything below this line is middleware (app) ----

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MondakiComics API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();