FROM ubuntu:24.04
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates libssl3t64 libstdc++6 zlib1g \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --chown=65532:65532 api/ ./
ENV SINGULARITY_HOST=0.0.0.0 SINGULARITY_PORT=8080
USER 65532:65532
EXPOSE 8080
ENTRYPOINT ["/app/singularity-api"]
