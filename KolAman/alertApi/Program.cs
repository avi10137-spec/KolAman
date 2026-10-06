using alertApi.Maping;
using alertApi.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AlertDbContext>(options =>
 options.UseMySql(
 builder.Configuration.GetConnectionString("alert_db"),
 ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("alert_db"))));
builder.Services.AddScoped<AlertRepository, AlertRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

