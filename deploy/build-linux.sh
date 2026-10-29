#!/bin/sh
# Runs locally in Docker using the official packaged compiler, never on the host platform.
set -eu
apt-get update -qq
apt-get install -y --no-install-recommends ca-certificates clang libssl3t64 libstdc++6 zlib1g >/dev/null
mkdir -p /opt/dream
tar -xzf /input/dream.tar.gz -C /opt/dream --strip-components=1
export PATH="/opt/dream:$PATH"
export DREAM_BIN=/opt/dream/dream
export DREAM_NO_AUTO_INSTALL=1
cd /work/singularity
dreamer install
dreamer pack -p singularity_api --target linux-x64
dream test be/src/backend_tests.dream
mkdir -p /out/api
cp be/target/pack/linux-x64/singularity_api-linux-x64 /out/api/singularity-api
# The compiler bundles the capability libraries beside the executable.
find be/target/pack/linux-x64 -maxdepth 1 -name '*.so*' -exec cp -P '{}' /out/api/ ';'
mkdir -p /out/api/public/target/web
cp fe/index.html fe/bootstrap.js /out/api/public/
cp -R fe/styles fe/assets /out/api/public/
cp fe/target/web/main.wasm fe/target/web/main.web.runtime.js /out/api/public/target/web/
sed -i 's|<head>|<head><meta name="singularity-api" content="same-origin">|' /out/api/public/index.html
chmod 755 /out/api/singularity-api
ldd /out/api/singularity-api
