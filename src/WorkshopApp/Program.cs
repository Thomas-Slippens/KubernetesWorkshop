var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();
var appName = Environment.GetEnvironmentVariable("APP_NAME") ?? "unknown";
var daprAppToken = Environment.GetEnvironmentVariable("DAPR_APP_TOKEN");

// ---------------------------------------------------------------
// Workshop participants: add your endpoints below this line!
// ---------------------------------------------------------------

app.MapGet("/", () => "Hello from the workshop! 👋");

app.MapGet("/hello", () => $"Hello from {appName}!");

app.MapGet("/internal/hello", (HttpRequest request) =>
{
    var suppliedToken = request.Headers["dapr-api-token"].ToString();
    return !string.IsNullOrEmpty(daprAppToken) && suppliedToken == daprAppToken
        ? Results.Ok($"Internal hello from {appName}! 👋")
        : Results.StatusCode(StatusCodes.Status403Forbidden);
});

app.MapGet("/call/{name}", async (string name, IHttpClientFactory factory) =>
{
    var client = factory.CreateClient();
    try
    {
        var url = $"http://localhost:3500/v1.0/invoke/{name}.workshop-{name}/method/internal/hello";
        var response = await client.GetStringAsync(url);
        return Results.Ok(new { from = appName, to = name, message = response });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Could not reach '{name}': {ex.Message}");
    }
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// Example: add your own endpoint
// app.MapGet("/hello", () => "My custom endpoint!");

// ---------------------------------------------------------------

app.Run();
