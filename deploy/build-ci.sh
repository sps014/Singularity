#!/usr/bin/env bash
set -euo pipefail
dreamer install
if ! dreamer test -p common --node; then
  trace="${RUNNER_TEMP:-/tmp}/dream-missing-files.log"
  strace -f -e trace=file -o "$trace" dreamer test -p common --node || true
  grep ENOENT "$trace" | tail -40 || true
  exit 1
fi
dream test be/src/backend_tests.dream
node --test fe/tests/session.test.mjs
dreamer build -p singularity --release
dreamer pack -p singularity_api --target linux-x64
sh deploy/stage.sh "$PWD/dist"
tar -czf dist/singularity-api-linux-x64.tar.gz -C dist/api .
(cd dist && sha256sum singularity-api-linux-x64.tar.gz > SHA256SUMS)
