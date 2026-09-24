# SoulsseekerWorkshopApp

A minimal .NET 9 WebAPI template for the Soulsseeker GitOps workshop.

> **Two repos are involved:**
> - **This repo** (your fork) — where you write code and push changes. You own this, no PRs needed.
> - **The infra repo** (`SoulsseekerInfra`) — the shared cluster config. You also fork this, then submit exactly **one PR** from your fork to register yourself.

## Workshop steps

1. **Fork** this repository on GitHub — this is your personal copy, you push directly to it
2. **Add secrets** to your fork: Settings → Secrets and variables → Actions → add `ACR_USERNAME` and `ACR_PASSWORD` (ask the workshop host for the credentials)
3. **Edit** `src/WorkshopApp/Program.cs` — add your own endpoints
4. **Push** to your fork — this triggers the `Build and Push` GitHub Actions workflow automatically (only fires when `src/` or `Dockerfile` changes)
   - Go to the **Actions** tab in your fork and confirm the workflow completes successfully
   - If you want to trigger it without a code change: Actions → **Build and Push** → **Run workflow**
   - The workflow builds your image, pushes it to ACR, and commits the updated `helm/values.yaml` back to your repo
   > ⚠️ Do not proceed to step 5 until the workflow has completed successfully — ArgoCD needs the image to exist in ACR before it can deploy
5. **Submit one PR** to the infra repo (`SoulsseekerInfra`):
   - Fork `https://github.com/Thomas-Slippens/SoulsseekerInfra` to your own GitHub account
   - Add the file `workshop/participants/<yourname>.yaml` on your fork:
   ```yaml
   name: <yourname>
   repoURL: https://github.com/<yourgithubusername>/KubernetesWorkshop
   ```
   > Make sure `repoURL` exactly matches your fork's real URL (copy it from GitHub). A mismatched or misspelled URL means ArgoCD silently deploys nothing.
   > `name` becomes your subdomain — keep it short and lowercase. This is the only PR you submit to the infra repo.
   - Open a PR from your fork back to `Thomas-Slippens/SoulsseekerInfra`
6. Once the workshop host merges your PR, watch ArgoCD deploy your app to `https://<yourname>.kubernetes.soulsseeker.com` 🚀

## Dapr internal-call exercise

The starter app includes a Dapr-protected `/internal/hello` endpoint. The workshop host provides its token centrally; do not create or commit a token yourself.

1. Visit `https://<yourname>.kubernetes.soulsseeker.com/internal/hello` — it returns `403 Forbidden` because the request did not come through Dapr.
2. Once Jurgen's app is deployed, visit `https://<yourname>.kubernetes.soulsseeker.com/call/jurgen` — your Dapr sidecar invokes Jurgen's `/internal/hello` endpoint successfully.
3. Read [the exercise guide](exercises/dapr-service-invocation/README.md) to understand the flow and adapt the endpoints.

## Local development

Run the app on its own (covers `/`, `/hello`, `/health`):

```sh
cd src/WorkshopApp
dotnet run --urls http://localhost:5000
# visit http://localhost:5000
```

> Without the `--urls` flag, `dotnet run` uses the ports from `Properties/launchSettings.json`.

Locally, `/internal/hello` always returns `403` (no `DAPR_APP_TOKEN` set) and `/call/{name}`
fails because there is no Dapr sidecar on `localhost:3500`. To exercise those endpoints, run
under Dapr.

### Run locally with Dapr

1. Install the [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/) and initialize the runtime:
   ```sh
   dapr init
   ```
2. Run the app under Dapr with a token so `/internal/hello` succeeds (Dapr's default HTTP port is `3500`):
   ```powershell
   $env:APP_NAME="me"; $env:DAPR_APP_TOKEN="localtoken"
   dapr run --app-id me --app-port 8080 --app-api-token localtoken `
     --dapr-http-port 3500 -- dotnet run --urls http://localhost:8080
   ```
   `--app-api-token` makes Dapr forward the `dapr-api-token` header to your app; it must match
   `DAPR_APP_TOKEN` for the `/internal/hello` check to pass.
3. To test `/call/{name}` locally, start a second app under the same Dapr instance with a
   different `--app-id`, and simplify the invoke address in `Program.cs` to just the app-id
   (the `name.workshop-name` form is a cluster/namespace concern).
