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
   repoURL: https://github.com/<yourgithubusername>/KubernernetesWorkshop
   ```
   > `name` becomes your subdomain — keep it short and lowercase. This is the only PR you submit to the infra repo.
   - Open a PR from your fork back to `Thomas-Slippens/SoulsseekerInfra`
6. Once the workshop host merges your PR, watch ArgoCD deploy your app to `https://<yourname>.kubernetes.soulsseeker.com` 🚀

## Dapr internal-call exercise

The starter app includes a Dapr-protected `/internal/hello` endpoint. The workshop host provides its token centrally; do not create or commit a token yourself.

1. Visit `https://<yourname>.kubernetes.soulsseeker.com/internal/hello` — it returns `403 Forbidden` because the request did not come through Dapr.
2. Once Jurgen's app is deployed, visit `https://<yourname>.kubernetes.soulsseeker.com/call/jurgen` — your Dapr sidecar invokes Jurgen's `/internal/hello` endpoint successfully.
3. Read [the exercise guide](exercises/dapr-service-invocation/README.md) to understand the flow and adapt the endpoints.

## Local development

```sh
cd src/WorkshopApp
dotnet run
# visit http://localhost:5000
```
