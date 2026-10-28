# Render Free backend

The GCP project `singluarity` was marked for deletion on October 28, 2026 after its Cloud Run service was removed. Render Free is the current deployment target. Keep the Render workspace without a payment method: Render documents that it suspends free services when bandwidth is exhausted instead of charging an overage.

Live backend: https://singularity-api-7ptj.onrender.com. Render service: `srv-dbgvge0u01pc73b5uh50`. The Hobby workspace has no card on file. Curl verified public health (200), anonymous catalog/search/audio/docs rejection (401), and invalid-token rejection (401). The packaged binary passed seven native backend tests and authenticated local curl checks before upload.

## Package locally

`package.ps1` uses Docker locally and the official, SHA-256-verified Dream v0.0.17 Linux x64 archive. It does not build the compiler. Download `dream-0.0.17-linux-x64.tar.gz` from the Dream GitHub release into `.verification/deploy/`, then run:

```powershell
./deploy/package.ps1
```

The output contains a relocatable Linux executable and its required Dream host libraries. `dist/Dockerfile` copies them into a non-root runtime container. Use `-OutputDirectory` to create another package without overwriting an existing release.

## Deploy

`render.yaml` declares the free service with a generated bearer token. Alternatively, select the public repository in the Render dashboard and set:

| Setting | Value |
| --- | --- |
| Runtime | Docker |
| Plan | Free |
| Region | Singapore |
| Dockerfile | `deploy/render.Dockerfile` |
| Health check | `/api/health` |
| `SINGULARITY_API_TOKEN` | Generate a random secret of at least 32 characters |
| `SINGULARITY_ORIGINS` | `https://frontend-not-configured.invalid` until the frontend exists |

The Dockerfile downloads the checksum-pinned application archive from GitHub Releases. Render installs runtime dependencies and assembles the container; it never compiles Dream or the application. Publish `singularity-api-linux-x64.tar.gz` under the pinned release tag before deploying. Update the tag and checksum together for later releases.

The app binds to Render's `PORT` with `SINGULARITY_HOST=0.0.0.0`. Missing/short secrets prevent public startup. All routes require `Authorization: Bearer <token>` except GET/HEAD `/api/health`. Do not embed this token in the browser frontend, Git or Docker image. A future browser frontend needs a user authentication or trusted server-side access design.

Render Free sleeps after 15 minutes without inbound traffic and may take about a minute to wake. There is no persistent backend library state; likes and playlists remain in the browser. Excess bandwidth and heavy external traffic may suspend the service. See [Render Free](https://render.com/docs/free) for the current rules.
