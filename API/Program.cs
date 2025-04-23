using System.Reflection;
using API.Data;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;

var entryAssembly = Assembly.GetEntryAssembly();

if (entryAssembly != null)
{
    var logRepository = LogManager.GetRepository(entryAssembly);
    XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
}
else
{
    // Handle fallback or log the issue
    Console.WriteLine("EntryAssembly is null. Log4net configuration may not be loaded.");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>( opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var serverTime = DateTime.UtcNow;
Console.WriteLine("Server Time: " + serverTime.ToString("yyyy-MM-dd HH:mm:ss"));
// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();
