using Lookup.WebApi.Endpoints;
using Lookup.WebApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

using var loggerFactory = LoggerFactory.Create(config => 
    config.SetMinimumLevel(LogLevel.Trace)
        .AddConsole()
);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.DescribeAllParametersInCamelCase();
});

builder.Services.AddSingleton<IWeatherRepository, WeatherRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseWeatherEndpoint();

app.Run();