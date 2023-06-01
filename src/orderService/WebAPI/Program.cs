using Application;
using Core.CrossCuttingConcerns.Exceptions.Extensions;
using Hangfire;
using Persistence;
using Swashbuckle.AspNetCore.SwaggerUI;
using WebAPI;
using WebAPI.Events;
using WebAPI.Jobs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices();

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();

//builder.Services.AddDistributedMemoryCache(); // InMemory
builder.Services.AddStackExchangeRedisCache(opt => opt.Configuration = "localhost:6379");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(
    opt =>
        opt.AddDefaultPolicy(p =>
        {
            p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        })
);
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc(name: "v1", info: new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Order Service" });
});

builder.Services.AddHostedService<TestBackgroundService>();

// Hangfire.AspNetCore, Hangfire.Core,
builder.Services.AddHangfire(
    configuration: config =>
        config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180) // Versiyon uyumluluk seviyesi
            .UseSimpleAssemblyNameTypeSerializer() // Assembly Serileştirme
            .UseRecommendedSerializerSettings() // Serileştirme
            .UseInMemoryStorage() // Hangfire.InMemory
            //.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")) // Hangfire.SqlServer
); // Configuration
builder.Services.AddHangfireServer(); // Hangfire Server

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.DocExpansion(DocExpansion.None);
    });
}

if (app.Environment.IsProduction())
    app.ConfigureCustomExceptionMiddleware();

app.UseHangfireDashboard(); // Hangfire Dashboard'ı ekleyen yapıyı kullanacağımızı belirtiyoruz.

app.MapControllers();
app.MapHangfireDashboard(); // localhost:5002/hangfire

const string webApiConfigurationSection = "WebAPIConfiguration";
WebApiConfiguration webApiConfiguration =
    app.Configuration.GetSection(webApiConfigurationSection).Get<WebApiConfiguration>()
    ?? throw new InvalidOperationException($"\"{webApiConfigurationSection}\" section cannot found in configuration.");
app.UseCors(opt => opt.WithOrigins(webApiConfiguration.AllowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials());

app.Services.ConfigureEventBusSubscriptions(app.Lifetime);

app.Run();
