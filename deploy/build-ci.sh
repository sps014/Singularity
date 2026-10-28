#!/usr/bin/env bash
set -euo pipefail
dreamer install
dreamer test -p common --node
dream test be/src/backend_tests.dream
node --test fe/tests/session.test.mjs
dreamer build -p singularity --release
dreamer pack -p singularity_api --target linux-x64
sh deploy/stage.sh "$PWD/dist"
tar -czf dist/singularity-api-linux-x64.tar.gz -C dist/api .
(cd dist && sha256sum singularity-api-linux-x64.tar.gz > SHA256SUMS)
