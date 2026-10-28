# Singularity API

Native Dream service using `system.webapi` and `youtube_explode`. One `YoutubeClient` shares its HTTP connection pools across search, metadata and audio operations.

Run from the repository root in separate terminals:

```powershell
./dev.ps1 run -Service backend -Release
./dev.ps1 run -Release
```

API: `http://127.0.0.1:8792`, with interactive documentation at `/docs`. Frontend: `http://127.0.0.1:8791/index.html`. Dependencies use the sibling `dream-packages/youtube_explode` checkout. Apply the toolchain patch described in the root README before building with an unpatched Dream checkout.

| Method | Path | Response |
| --- | --- | --- |
| GET | `/api/health` | Process health, without upstream requests |
| GET | `/api/catalog` | Four playable demo tracks |
| GET | `/api/search?q=...&continuation=...` | YouTube video tracks and an optional continuation token |
| GET | `/api/tracks/{id}` | One video's metadata |
| GET | `/api/audio/{id}` | Seekable audio byte ranges |
| HEAD | `/api/audio/{id}` | Audio MIME type, length and range support |

Search accepts 2–200 characters; continuation tokens are opaque, limited to 8192 characters, and reused with the original query. Search returns metadata without resolving audio for every result. Playback resolves only the selected track. Track IDs are `yt:<video-id>`; endpoints take the raw video ID.

Audio supports closed, open-ended and suffix `Range` requests. Responses are at most 1 MiB, with status `206`, `Content-Range`, `Content-Length` and `Accept-Ranges: bytes`. The media element resumes at the next byte, avoiding whole-song buffering. Invalid, unsatisfiable and multipart ranges return `416`. Requests without `Range` receive a non-cacheable `307` to the temporary upstream URL; the browser player sends range requests and uses the API throughout playback. AAC/MP4 is preferred, followed by WebM/Opus. Live-only HLS streams are unsupported.

Errors use `{"error":{"code":"...","message":"..."}}`: invalid input `400`, unavailable audio `422`, rate limiting `429`, upstream failure `502`, capacity exhaustion `503`. Admission and rate-limit responses include `Retry-After`. Generated parameter validation uses WebApp's standard `422` format. Upstream error text is not reflected because it can include signed URLs or cookies.

Caches hold at most 128 search pages (2-minute TTL), 256 metadata records (10-minute TTL), and 64 streams (1-minute TTL). Oldest entries are evicted; signed URLs are never persisted. At most 8 uncached upstream operations and 16 range reads run concurrently; excess work receives `503` without blocking the scheduler. Permits release on completion and cancellation. Request cancellation reaches YouTube HTTP operations, which use the package's deadlines and retry policy. The server caps connections/in-flight requests at 64, keep-alive at 5 seconds and graceful shutdown at 5 seconds; forwarded proxy headers are disabled.

| Environment variable | Default |
| --- | --- |
| `SINGULARITY_HOST` | `127.0.0.1` |
| `SINGULARITY_PORT` | `8792` |
| `SINGULARITY_ORIGINS` | `http://127.0.0.1:8791,http://localhost:8791` |
| `SINGULARITY_API_TOKEN` | Empty for loopback development; required on public bindings |

Public bindings require a 32–200-character bearer token. Supply it as `Authorization: Bearer <token>`; URL tokens are rejected. All routes, including `/docs`, require authorization except `GET /api/health` for platform health checks. CORS permits the Authorization header for configured origins. Generate a random token and keep it in host environment settings; never put it in frontend code, Git or an image. `PORT` is supported when `SINGULARITY_PORT` is absent.

The helper's `-ApiPort` sets the API port. The frontend defaults to its current hostname and protocol with port 8792; override with `<meta name="singularity-api" content="https://your-api.example">` in its HTML host and configure the matching CORS origin.

`routes.dream` handles HTTP extraction and validation; `service.dream` owns upstream operations; `mapping.dream` converts metadata and selects formats; `audio.dream` reads bounded ranges; `ranges.dream` parses byte ranges; `responses.dream` maps errors; `cache.dream` and `permit.dream` implement resource policies. `main.dream` configures and starts WebApp. Shared models live in `common`.

```powershell
./dev.ps1 test
./dev.ps1 build -Service backend -Release
```

Seven offline backend tests cover ranges/overflow, cache expiry/eviction, permit lifetime, audio selection, error redaction, actual loopback HTTP routing and bearer-token enforcement. Tests are colocated at `src/backend_tests.dream` so Dream imports the source modules directly. The root helper runs them natively and runs shared library tests on Node.

Likes, playlists and saved track metadata remain browser-local. Accounts, server-side library storage, cloud synchronization, uploads and transcoding are outside this version. YouTube extraction and availability can change independently; failures are explicit in the player.
