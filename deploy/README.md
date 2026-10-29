# Render Free music player

The GCP project `singluarity` was marked for deletion on October 28, 2026 after its Cloud Run service was removed. Render Free is the current deployment target. Keep the Render workspace without a payment method: Render documents that it suspends free services when bandwidth is exhausted instead of charging an overage.

Live app: https://singularity-api-7ptj.onrender.com. Render service: `srv-dbgvge0u01pc73b5uh50`. The frontend and API share this origin. Access is public, with no key prompt; the backend enforces request, response-bandwidth and concurrency limits. See [backend protections](../be/README.md) for defaults and limits.

## Package locally

`package.ps1` uses the existing local Dream toolchain for the portable browser WASM, then Docker and the official, SHA-256-verified Dream v0.0.17 Linux x64 archive for the backend. It does not build either compiler. Download `dream-0.0.17-linux-x64.tar.gz` from the Dream GitHub release into `.verification/deploy/`, then run:

```powershell
./deploy/package.ps1
```

The output contains a relocatable Linux executable and its required Dream host libraries. `dist/Dockerfile` copies them into a non-root runtime container. Use `-OutputDirectory` to create another package without overwriting an existing release.

## Deploy

`render.yaml` declares the public free service. Alternatively, select the public repository in the Render dashboard and set:

| Setting | Value |
| --- | --- |
| Runtime | Docker |
| Plan | Free |
| Region | Singapore |
| Dockerfile | `deploy/render.Dockerfile` |
| Health check | `/api/health` |
| `SINGULARITY_ORIGINS` | `https://singularity-api-7ptj.onrender.com` |

The Dockerfile uses Render's `RENDER_GIT_COMMIT` build argument to download the matching `app-<commit>` release and verifies its SHA-256 checksum. Render installs runtime dependencies and assembles the container; it never compiles Dream or the application. CI publishes the release only after all tests and builds pass, then triggers that exact commit.

The app binds to Render's `PORT` with `SINGULARITY_HOST=0.0.0.0`. The frontend is packaged under `public/` and uses same-origin API/audio URLs. No API token or browser session is required; any old `SINGULARITY_API_TOKEN` environment value is unused. Likes and playlists stay in local browser storage.

The public guard returns 429 with retry guidance for excessive request volume or response bandwidth. Search/audio cost more admission units than static pages. Existing caches, fixed video-ID routes, 1 MiB audio ranges and smaller concurrency ceilings bound upstream work and memory. These in-memory limits reset on process restart; they are not a persistent usage quota. Forwarded headers are ignored rather than trusting client-provided IP addresses. Proxy peers can share a throttle bucket.

Render Free sleeps after 15 minutes without inbound traffic and may take about a minute to wake. There is no persistent backend library state; likes and playlists remain in the browser. Excess bandwidth and heavy external traffic may suspend the service. See [Render Free](https://render.com/docs/free) for the current rules.

## Continuous deployment

`.github/workflows/deploy.yml` pins the Dream v0.0.19 release containing the ARC improvements and standalone browser runtime fixes, and the dependency repository commit. It downloads the verified compiler archive, uses its bundled minimal LLVM and prebuilt runtime, and installs Binaryen through Dreamer. It runs shared and backend tests, builds both applications and publishes the compiled package. No compiler is built in this pipeline.

Pushes to `main` deploy only after successful validation. Pull requests build/test without receiving deployment secrets. `workflow_dispatch` retries the pipeline manually. The deployment check waits until `/api/health` reports the tested commit and checks the frontend, WASM and public catalog access.

Set `RENDER_DEPLOY_HOOK` under GitHub repository Settings → Secrets and variables → Actions. To save it without displaying the value in terminal output or chat:

```powershell
./deploy/save-render-hook.ps1
```

Keep Render auto-deploy off so it cannot start before CI publishes the matching package.

Linux deployment packages use `--release -O 2`: Dream's O3 host builds tune instructions to the build CPU, which may differ from Render's CPU. CI starts the exact packaged executable under QEMU's baseline `qemu64` CPU and checks public assets, catalog access and throttling before publishing.
