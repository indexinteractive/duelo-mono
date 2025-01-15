FROM debian:bookworm-slim AS build

RUN apt-get update && apt-get install -y \
    libz-dev \
    libsecret-1-dev \
    libglib2.0-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

FROM debian:bookworm-slim AS mpuk

RUN groupadd --gid 2000 mpukgame && \
    useradd -g 2000 -u 2000 -ms /bin/sh mpukgame && \
    mkdir /game && \
    chown mpukgame:mpukgame /game

USER mpukgame
WORKDIR /game

COPY --from=build \
    /usr/lib/x86_64-linux-gnu/libz.so.1 \
    /usr/lib/x86_64-linux-gnu/libsecret-1.so.0 \
    /usr/lib/x86_64-linux-gnu/libglib-2.0.so.0 \
    /usr/lib/x86_64-linux-gnu/libstdc++.so.6 \
    /usr/lib/x86_64-linux-gnu/libgio-2.0.so.0 \
    /usr/lib/x86_64-linux-gnu/libgobject-2.0.so.0 \
    /usr/lib/x86_64-linux-gnu/libgmodule-2.0.so.0 \
    /usr/lib/x86_64-linux-gnu/

COPY --chown=mpukgame ./server_linux/ /game/
COPY --chown=mpukgame ./server.json /home/mpukgame/server.json

ENTRYPOINT ["/game/duelo-server-0.0.0.0"]