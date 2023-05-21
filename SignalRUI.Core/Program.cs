using Microsoft.EntityFrameworkCore;
using SignalRUI.Core.Hubs;
using SignalRUI.Core.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(configuration["ConnectionStrings:Sql"]);
});
builder.Services.AddSignalR();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("https://localhost:7213").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthorization();

app.MapControllers();

// http://localhost:4400/MyHub
app.MapHub<MyHub>("/MyHub");
app.Run();
