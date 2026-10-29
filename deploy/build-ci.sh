#!/usr/bin/env bash
set -euo pipefail
dreamer install
dreamer test -p common --node
dream test be/src/backend_tests.dream
dream test fe/src/stream_error_tests.dream
dreamer build -p singularity --release
dreamer pack -p singularity_api --release -O 2 --target linux-x64
sh deploy/stage.sh "$PWD/dist"
SINGULARITY_PACKAGE_EMULATOR=qemu-x86_64 sh deploy/check-package.sh "$PWD/dist/api"
tar -czf dist/singularity-api-linux-x64.tar.gz -C dist/api .
(cd dist && sha256sum singularity-api-linux-x64.tar.gz > SHA256SUMS)
