# Render Free backend

The GCP project `singluarity` was marked for deletion on October 28, 2026 after its Cloud Run service was removed. Render Free is the current deployment target. Keep the Render workspace without a payment method: Render documents that it suspends free services when bandwidth is exhausted instead of charging an overage.

Live backend: https://singularity-api-7ptj.onrender.com. Render service: `srv-dbgvge0u01pc73b5uh50`. The Hobby workspace has no card on file. Curl verified public health (200), anonymous catalog/search/audio/docs rejection (401), and invalid-token rejection (401). The packaged binary passed seven native backend tests and authenticated local curl checks before upload.

## Package locally

`package.ps1` uses the existing local Dream toolchain for the portable browser WASM, then Docker and the official, SHA-256-verified Dream v0.0.17 Linux x64 archive for the backend. It does not build either compiler. Download `dream-0.0.17-linux-x64.tar.gz` from the Dream GitHub release into `.verification/deploy/`, then run:

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

The Dockerfile uses Render's `RENDER_GIT_COMMIT` build argument to download the matching `app-<commit>` release and verifies its SHA-256 checksum. Render installs runtime dependencies and assembles the container; it never compiles Dream or the application. CI publishes the release only after all tests and builds pass, then triggers that exact commit.

The app binds to Render's `PORT` with `SINGULARITY_HOST=0.0.0.0`. Missing/short secrets prevent public startup. API requests require a bearer token or the private browser cookie, except GET/HEAD `/api/health`. The frontend is packaged under `public/` in the same service and uses same-origin API/audio URLs. On the sign-in page, enter the existing Render `SINGULARITY_API_TOKEN` value; it is never embedded in public assets. Sign-out clears the HttpOnly cookie. This is private access for a small shared deployment, not individual user accounts or cloud synchronization.

The packaged Dream HTTP host intentionally omits browser credentials. `fe/session.js` enables cookies only for the packaged deployment's same-origin `/api/` calls. External audio and image requests retain their original credential settings. Test that boundary with `node --test fe/tests/session.test.mjs`.

Render Free sleeps after 15 minutes without inbound traffic and may take about a minute to wake. There is no persistent backend library state; likes and playlists remain in the browser. Excess bandwidth and heavy external traffic may suspend the service. See [Render Free](https://render.com/docs/free) for the current rules.

## Continuous deployment

`.github/workflows/deploy.yml` pins the Dream v0.0.18 release containing the ARC improvements and the dependency repository commit. It downloads the verified compiler archive and pinned LLVM/Binaryen binaries, runs shared/backend/browser-boundary tests, builds both applications and publishes the compiled package. No compiler is built in this pipeline.

Pushes to `main` deploy only after successful validation. Pull requests build/test without receiving deployment secrets. `workflow_dispatch` retries the pipeline manually. The deployment check waits until `/api/health` reports the tested commit and checks the frontend, WASM and unauthorized API rejection.

Set `RENDER_DEPLOY_HOOK` under GitHub repository Settings → Secrets and variables → Actions. To save it without displaying the value in terminal output or chat:

```powershell
./deploy/save-render-hook.ps1
```

The player access token stays in Render environment settings and is not required by the build. Keep Render auto-deploy off so it cannot start before CI publishes the matching package.
