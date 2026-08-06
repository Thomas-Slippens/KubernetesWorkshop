// This is a reference copy of Program.cs for the Dapr service invocation exercise.
// Copy the two new endpoints into your src/WorkshopApp/Program.cs.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();

// Your pod's name — injected automatically by the platform via Helm
var appName = Environment.GetEnvironmentVariable("APP_NAME") ?? "unknown";
var daprAppToken = Environment.GetEnvironmentVariable("DAPR_APP_TOKEN");

// ---------------------------------------------------------------
// Workshop participants: add your endpoints below this line!
// ---------------------------------------------------------------

app.MapGet("/", () => "Hello from the workshop! 👋");

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// --- Exercise: Dapr service invocation ---

// Step 1: expose /hello as a normal public endpoint
app.MapGet("/hello", () => $"Hello from {appName}! 👋");

// Step 2: expose an endpoint that accepts requests forwarded by Dapr only
app.MapGet("/internal/hello", (HttpRequest request) =>
{
    var suppliedToken = request.Headers["dapr-api-token"].ToString();
    return !string.IsNullOrEmpty(daprAppToken) && suppliedToken == daprAppToken
        ? Results.Ok($"Internal hello from {appName}! 👋")
        : Results.StatusCode(StatusCodes.Status403Forbidden);
});

// Step 3: call another participant's internal endpoint via Dapr
// Try it: https://<yourname>.kubernetes.soulsseeker.com/call/jurgen
app.MapGet("/call/{name}", async (string name, IHttpClientFactory factory) =>
{
    var client = factory.CreateClient();
    try
    {
        // Dapr service invocation format: <appId>.<namespace>
        var url = $"http://localhost:3500/v1.0/invoke/{name}.workshop-{name}/method/internal/hello";
        var response = await client.GetStringAsync(url);
        return Results.Ok(new { from = appName, to = name, message = response });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Could not reach '{name}': {ex.Message}");
    }
});

// ---------------------------------------------------------------

app.Run();
