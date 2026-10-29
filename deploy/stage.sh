#!/bin/sh
set -eu
output="${1:?output directory required}"
mkdir -p "$output/api/public/target/web"
cp be/target/pack/linux-x64/singularity_api-linux-x64 "$output/api/singularity-api"
find be/target/pack/linux-x64 -maxdepth 1 -name '*.so*' -exec cp -P '{}' "$output/api/" ';'
cp fe/index.html fe/bootstrap.js "$output/api/public/"
cp -R fe/styles fe/assets "$output/api/public/"
cp fe/target/web/main.wasm fe/target/web/main.web.runtime.js "$output/api/public/target/web/"
sed -i 's|<head>|<head><meta name="singularity-api" content="same-origin">|' "$output/api/public/index.html"
chmod 755 "$output/api/singularity-api"
