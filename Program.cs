var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

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
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/health", () => "OK");

app.MapPost("/products", (Product product) =>
{
    return product;
});

app.MapGet("/environment", () => app.Environment.EnvironmentName);

app.MapGet("/appname", () =>
{
    var appName = Environment.GetEnvironmentVariable("APP_NAME");
    return appName;
});

app.MapGet("/db-test", async () =>
{
    var host = Environment.GetEnvironmentVariable("DB_HOST");
    var port = Environment.GetEnvironmentVariable("DB_PORT");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var user = Environment.GetEnvironmentVariable("DB_USER");
    var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

    var connectionString =
        $"Host={host};Port={port};Database={dbName};Username={user};Password={password}";

    await using var connection = new Npgsql.NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    return "Database bağlantısı başarılı!";
});

app.MapGet("/products", async () =>
{
    var host = Environment.GetEnvironmentVariable("DB_HOST");
    var port = Environment.GetEnvironmentVariable("DB_PORT");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var user = Environment.GetEnvironmentVariable("DB_USER");
    var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

    var connectionString =
        $"Host={host};Port={port};Database={dbName};Username={user};Password={password}";

    await using var connection = new Npgsql.NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    var command = new Npgsql.NpgsqlCommand(
        "SELECT id, name FROM products",
        connection
    );

    await using var reader = await command.ExecuteReaderAsync();

    var products = new List<object>();

    while (await reader.ReadAsync())
    {
        products.Add(new
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1)
        });
    }

    return products;
});

app.Run();

record Product(string Name);


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
