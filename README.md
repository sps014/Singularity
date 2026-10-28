# Singularity

A browser music player with dark acrylic surfaces, a responsive library and Dream-powered audio. The workspace has three source folders: `fe` for the browser app, `be` for the native YouTube API, and `common` for browser-independent models and library rules.

For the packaged backend on Render Free, see [deployment instructions](deploy/README.md). Public deployments require a bearer token; local loopback development remains available without one.

## Run locally

Use the Dream checkout next to this repository. If it does not yet include the synthetic-future JS-marshaling fix, apply the bundled patch once from this repository:

```powershell
$patch = (Resolve-Path ./fe/toolchain/dream-js-marshal.patch).Path
git -C ../dream apply --check $patch
git -C ../dream apply $patch
```

Skip that step when your Dream checkout already includes the fix. The backend also needs the generator-depth, generic type-identity, cycle-analysis and browser diagnostic fixes bundled in `be/toolchain/dream-backend.patch`. Apply it once to an unpatched checkout:

```powershell
$patch = (Resolve-Path ./be/toolchain/dream-backend.patch).Path
git -C ../dream apply --check $patch
git -C ../dream apply $patch
```

Keep the `dream-packages` checkout beside this repository; `be/dream.toml` points to its `youtube_explode` package. Build the current tools after applying the patches:

```powershell
cd ../dream
cargo build --release -p dream -p dreamer
cd ../singularity
./dev.ps1 install
./dev.ps1 run -Service backend -Release
# In a second terminal:
./dev.ps1 run
```

Open http://127.0.0.1:8791/index.html. The PowerShell helper sets `DREAM_BIN` to the local source build and invokes that checkout's `dreamer`. Pass `-DreamSource` to use another checkout or `-Port` to change the server port.

```powershell
./dev.ps1 build
./dev.ps1 build -Release
./dev.ps1 test
```

Equivalent commands with the toolchain already selected:

```text
dreamer install
dreamer run -p singularity --port 8791
dreamer build -p singularity --release
dreamer test --node
```

The root `dream.toml` registers `fe`, `be` and `common`, with path dependencies and one shared lockfile. Generated WASM and the selective browser runtime live in `fe/target/web`; keep the `fe` directory as the web server root. Opening `index.html` directly from disk is unsupported.

## Use the player

Discover the catalog, filter by mood, or search songs, artists, albums and playlist names. Heart songs to keep them in Liked Songs. Use a song's plus button to add it to a playlist; create and edit playlists from the library. Collection playback builds the queue in collection order. Previous restarts a song after three seconds; otherwise it goes to the previous song. Shuffle preserves the current song; repeat cycles off → all → one. Automatic playback stops at the end when repeat is off. Manual next/previous wraps the queue.

On mobile, tap the mini-player to open the full player. On tablet, the queue button opens the player panel. Space toggles playback outside editable controls; `/` focuses search and Escape closes the expanded player. Playback starts with user interaction as required by browsers.

Likes and playlists are saved under `singularity.library.v1` in this browser's local storage. No account or cloud synchronization is used. Unknown track references are discarded on restore; unreadable data starts an empty library. If browser storage is blocked or full, the app remains usable for the session. The first visit includes two editable example playlists.

## Code boundaries

- `common/src`: track and playlist models, catalog search, library mutations and queue behavior. Tests live in `common/tests` and run through Dream's Node test host.
- `fe/src`: state, DOM utilities, shell, browsing views, player rendering, events, storage and audio coordination. Dream generates the interface and uses `system.audio.Audio` for all playback.
- `fe/styles`: design tokens, responsive layout and components. `fe/assets` contains original SVG artwork, bundled locally.
- `fe/bootstrap.js`: WASM loading and a small storage exception/syntax boundary. Dream's dynamic JS interop cannot catch browser storage exceptions, so this boundary returns explicit success/failure values and rejects malformed JSON before it reaches the Dream parser. Serialization, schema validation and library recovery remain in Dream.
- `be/src`: native `system.webapi` routes, YouTube service, bounded caches, audio range handling and tests. See [backend documentation](be/README.md) for endpoints, configuration and resource limits.

## Demo audio

The demo uses four files from [MDN's Web Audio examples](https://github.com/mdn/webaudio-examples): Outfoxing the Fox, Viper, an R&B lo-fi melody loop, and the Bass Guitar multitrack example. [Outfoxing the Fox is credited to Kevin MacLeod](https://github.com/mdn/webaudio-examples/blob/main/audio-basics/index.html); the multitrack example [credits jplayer.org as its source](https://github.com/mdn/webaudio-examples/blob/main/multi-track/index.html). Other artist names are left unknown rather than guessed. The interface links to the examples and their credits. Artwork and mood tags are original presentation choices, not official album artwork.

All four demo URLs were verified for CORS and HTTP byte-range responses, including cold browser loads and forward/backward seeks. Chrome's Media Session demo URLs were replaced because their CDN sometimes returned full `200` responses to range requests and the browser exposed only a zero-length seekable range; native browser seeking failed too. No compiler or audio-runtime change was made for that source-side failure. Streams require an internet connection. Change sources and metadata in `common/src/catalog.dream`.

The local Dream checkout includes a small JS-marshaling fix required by cancellable audio operations: synthetic future types without object layouts are excluded from generated marshalers. The fix and its regression test are included in `fe/toolchain/dream-js-marshal.patch`; the policy lives in `crates/dream-mir/src/backend/shared/js_marshal.rs` in the sibling Dream repository.

Search now queries YouTube through the native backend, with a 350 ms debounce, stale-request cancellation and pagination. Liked and playlist tracks retain their metadata across reloads. Accounts, cloud synchronization, uploads, transcoding and audio visualization remain outside this version.
