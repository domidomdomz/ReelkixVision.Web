using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using ReelkixVision.Web.Application.Interfaces;
using ReelkixVision.Web.Infrastructure.AWS;
using ReelkixVision.Web.Infrastructure.ExternalServices;
using ReelkixVision.Web.Infrastructure.FeatureFlags;
using ReelkixVision.Web.Infrastructure.Persistence;
using ReelkixVision.Web.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Load settings from appsettings.json, then override with environment-specific files
builder.Configuration
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();

// Retrieve the allowed origins from the "CorsSettings:AllowedOrigins" configuration section.
var allowedOrigins = builder.Configuration
    .GetSection("CorsSettings:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    // Use AllowAnyOrigin if no origins are defined or if "*" is specified.
    if (allowedOrigins == null || allowedOrigins.Length == 0 || (allowedOrigins.Length == 1 && allowedOrigins[0] == "*"))
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyOrigin()   // Allow requests from any origin.
                  .AllowAnyHeader()   // Allow any header.
                  .AllowAnyMethod();  // Allow any HTTP method.
        });
    }
    else
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.WithOrigins(allowedOrigins) // Allow only the specified origins.
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    }
});


// Add services to the container.
// Add AWS S3 client via AWS SDK DI Extensions.
builder.Services.AddAWSService<IAmazonS3>();

// Register project services.
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IFeatureFlagService, FeatureFlagService>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddHttpClient<IAnalysisService, ReelkixVisionAnalysisService>();

// Configure EF Core (using a free/local SQL Server instance, adjust as needed).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use the CORS policy
app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
