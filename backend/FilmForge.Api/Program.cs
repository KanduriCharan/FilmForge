using FilmForge.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Amazon;
using Amazon.S3;
using FilmForge.Api.Services.Media;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddDbContext<AppDbContext>( options => 
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var config = builder.Configuration;
    var accessKey = config["AWS:AccessKey"];
    var secretKey = config["AWS:SecretKey"];
    var region = config["AWS:Region"] ?? "us-east-1";

    return new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
});

builder.Services.AddScoped<IS3MediaService, S3MediaService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();