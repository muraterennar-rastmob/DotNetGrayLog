using Microsoft.AspNetCore.Mvc;

namespace GrayLogTutorial.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet("GetWeatherForecast/{isError}")]
    public async Task<IActionResult> Get([FromRoute] bool isError)
    {
        // Simülasyon amaçlı bekleme süresi eklenmiş
        await Task.Delay(100); // Örneğin 100ms'lik bir gecikme ekliyoruz

        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        if (isError)
            throw new Exception("Custom Error"); // Hata Middleware'e gider

        return Ok(result);
    }
}