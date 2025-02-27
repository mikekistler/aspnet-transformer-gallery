using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(new InfoContactTransformer());

    TypeTransformer.MapType<decimal>(new OpenApiSchema { Type = "number", Format = "decimal" });
    options.AddSchemaTransformer(TypeTransformer.TransformAsync);
    options.AddNullableTransformer();
    options.AddSecuritySchemeTransformer();
    options.AddProblemResponseTransformer();
    options.AddDocumentTransformer<CanonicalDocumentTransformer>();
});

builder.Services.AddAuthorization();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)],
            (decimal)Random.Shared.NextDouble()
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapPost("/snow-day", () =>
{
    return Results.Ok("No school today!");
})
.WithName("SnowDay")
.RequireAuthorization();

app.MapPost("/nullable-props", (Body body) =>
{
    return TypedResults.Ok(body);
});

app.MapPost("/command", (Command body) =>
{
    return TypedResults.Ok(body);
});

app.MapPost("/tag", (Tag body) =>
{
    return TypedResults.Ok(body);
});

app.MapGet("/problems",
    Results<Ok<string>, ProblemHttpResult> (int status) =>
{
    switch (status)
    {
        case 404:
            return TypedResults.Problem(new (){
                Status = 404,
                Detail = "Resource not found"
            });
        case 409:
            return TypedResults.Problem(new (){
                Status = 409,
                Detail = "Conflict"
            });
        default:
            return TypedResults.Problem(new (){
                Status = 400,
                Detail = "request is not valid"
            });
    }
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, decimal chanceOfPrecip)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class Body
{
    required public string Name { get; set; }
    public Address HomeAddress { get; set; }
    public Address? WorkAddress { get; set; }
    required public string PhoneNumber { get; set; }
    public PhoneType PhoneType { get; set; }
    public string? AltPhoneNumber { get; set; }
    public PhoneType? AltPhoneType { get; set; }
}

public struct Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PhoneType
{
    Home,
    Work,
    Mobile
}

public sealed record Command(string Name, string? Description, ETagNamespace? Namespace = null);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ETagNamespace
{
	ContentWarning = 1,
	Genre = 2,
	Franchise = 3,
}

public sealed class Tag
{
	public string? Description { get; init; }
	public ETagNamespace? Namespace { get; init; }
}
