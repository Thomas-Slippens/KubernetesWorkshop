# Exercise: Dapr Service Invocation

Call another participant's internal endpoint by name — no URLs, no namespace knowledge needed.

## What is happening

Each pod has a **Dapr sidecar** injected automatically. To call another participant's app,
you talk to your own sidecar on `localhost:3500`. Dapr handles routing across namespaces.

The platform also provides an `app-api-token` Secret in every workshop namespace. Dapr
adds this token when it forwards a request to the app; the app rejects direct requests to
its `/internal/*` endpoint.

```
Your app → Your Dapr sidecar → Jurgen's Dapr sidecar → Jurgen's app
```

## What to add to Program.cs

Copy the two endpoints below into your `src/WorkshopApp/Program.cs`,
below the existing endpoints.

See [Program.cs](./Program.cs) for the complete example.

## How to test

Once deployed, open your app in the browser:

| URL | What it does |
|-----|-------------|
| `https://<yourname>.kubernetes.soulsseeker.com/hello` | Returns a public greeting from your pod |
| `https://<yourname>.kubernetes.soulsseeker.com/internal/hello` | Returns `403 Forbidden` because it was not forwarded by Dapr |
| `https://<yourname>.kubernetes.soulsseeker.com/call/jurgen` | Calls Jurgen's `/internal/hello` via Dapr and returns his response |

Try calling each other's pods once everyone is deployed!
