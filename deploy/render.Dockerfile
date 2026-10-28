FROM ubuntu:24.04
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates curl libssl3t64 libstdc++6 zlib1g \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
# CI publishes a release for this exact commit before it triggers Render.
ARG RENDER_GIT_COMMIT
RUN test -n "$RENDER_GIT_COMMIT" \
    && cd /tmp \
    && curl -fLsS --retry 3 -O "https://github.com/sps014/Singularity/releases/download/app-${RENDER_GIT_COMMIT}/singularity-api-linux-x64.tar.gz" \
    && curl -fLsS --retry 3 -O "https://github.com/sps014/Singularity/releases/download/app-${RENDER_GIT_COMMIT}/SHA256SUMS" \
    && sha256sum --check --strict SHA256SUMS \
    && tar -xzf singularity-api-linux-x64.tar.gz -C /app \
    && chmod 755 /app/singularity-api \
    && rm singularity-api-linux-x64.tar.gz SHA256SUMS
ENV SINGULARITY_HOST=0.0.0.0
USER 65532:65532
EXPOSE 10000
ENTRYPOINT ["/app/singularity-api"]
