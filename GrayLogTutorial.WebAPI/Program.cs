using GrayLogTutorial.Core.ExceptionHandling;
using GrayLogTutorial.Core.Logs;
using GrayLogTutorial.Core.Logs.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Graylog(new GraylogSinkOptions()
    {
        HostnameOrAddress = builder.Configuration["Graylog:Host"],
        Port = Int32.Parse(builder.Configuration["Graylog:Port"]),
        Facility = builder.Configuration["Graylog:Facility"]
    })
    .CreateLogger();

builder.Services.AddSingleton<ILoggerService, SerilogLoggerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();