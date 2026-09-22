using System.Text.Json;
using System.Text.Json.Serialization;
using Dengue.Api;
using Dengue.Application.Interfaces;
using Dengue.Application.Services;
using Dengue.Infrastructure.Context;
using Dengue.Infrastructure.External;
using Dengue.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

//Carrega o .env da raiz do projeto
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env");
if (File.Exists(envPath))
    Env.Load(envPath);

//Controllers
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Banco de dados
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DengueDbContext>(options => 
    options.UseSqlServer(connectionString)
);

//Options
builder.Services.Configure<AlertaDengueOptions>(
    builder.Configuration.GetSection(AlertaDengueOptions.SectionName)
);

//Repositório e serviço
builder.Services.AddScoped<IDengueRepository, DengueRepository>();
builder.Services.AddScoped<IDengueService, DengueService>();

//Cliente HTTP
builder.Services.AddHttpClient<IAlertaDengueClient, AlertaDengueClient>(client => {
   var baseUrl = builder.Configuration["AlertaDengue:BaseUrl"] ?? "https://info.dengue.mat.br/api/alertcity";
   client.BaseAddress = new Uri(baseUrl);
   client.Timeout = TimeSpan.FromSeconds(60); 
});

// Hosted service (popula o banco no startup se estiver vazio, ou toda segunda as 3 da manhã) 
builder.Services.AddHostedService<DengueSyncHostedService>();

//CORS
builder.Services.AddCors(options => {
    options.AddPolicy("Frontend", policy => policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if(app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();