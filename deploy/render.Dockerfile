FROM ubuntu:24.04
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates libssl3t64 libstdc++6 zlib1g \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
# Download only the packaged application, never a compiler or source build.
ADD --checksum=sha256:6579a86a3791e9d72b171912daf4c7c810ff65d52b4d08fd7cc523f72cfa3744 https://github.com/sps014/Singularity/releases/download/backend-v0.1.0/singularity-api-linux-x64.tar.gz /tmp/api.tar.gz
RUN tar -xzf /tmp/api.tar.gz -C /app \
    && chmod 755 /app/singularity-api \
    && rm /tmp/api.tar.gz
ENV SINGULARITY_HOST=0.0.0.0
USER 65532:65532
EXPOSE 10000
ENTRYPOINT ["/app/singularity-api"]
