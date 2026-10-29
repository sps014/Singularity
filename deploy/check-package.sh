#!/bin/sh
set -eu
package="${1:?package directory required}"
log="${RUNNER_TEMP:-/tmp}/singularity-package-smoke.log"
port=18798
(
  cd "$package"
  export SINGULARITY_HOST=127.0.0.1 SINGULARITY_PORT="$port"
  if [ -n "${SINGULARITY_PACKAGE_EMULATOR:-}" ]; then
    exec "$SINGULARITY_PACKAGE_EMULATOR" -cpu qemu64 ./singularity-api
  fi
  exec ./singularity-api
) >"$log" 2>&1 &
pid=$!
trap 'kill "$pid" 2>/dev/null || true; wait "$pid" 2>/dev/null || true' EXIT INT TERM
base="http://127.0.0.1:$port"
attempt=0
until curl --fail --silent --max-time 2 "$base/api/health" > /dev/null; do
  if ! kill -0 "$pid" 2>/dev/null || [ "$attempt" -ge 30 ]; then
    cat "$log"
    echo 'Packaged backend did not start on the baseline CPU.'
    exit 1
  fi
  attempt=$((attempt + 1))
  sleep 1
done
for path in / /target/web/main.wasm /api/catalog; do
  test "$(curl --silent --max-time 10 -o /dev/null -w '%{http_code}' "$base$path")" = 200
done
limited=false
attempt=0
while [ "$attempt" -lt 50 ]; do
  status=$(curl --silent --max-time 10 -o /dev/null -w '%{http_code}' "$base/api/search?q=x")
  if [ "$status" = 429 ]; then limited=true; break; fi
  test "$status" = 400
  attempt=$((attempt + 1))
done
test "$limited" = true
echo 'Portable package serves the public player and enforces request limits.'
