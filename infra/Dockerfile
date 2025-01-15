FROM ubuntu:24.04 AS mpuk

RUN groupadd --gid 2000 mpukgame && \
    useradd -g 2000 -u 2000 -ms /bin/sh mpukgame && \
    mkdir /game && \
    chown mpukgame:mpukgame /game

FROM mpuk AS game
USER root

ENV DEBIAN_FRONTEND=noninteractive
RUN apt-get update && apt-get install -y \
    libz-dev \
    libsecret-1-dev \
    libglib2.0-dev \
    libstdc++6 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

USER mpukgame
WORKDIR /game

COPY --chown=mpukgame ./server_linux/ /game/
COPY --chown=mpukgame ./server.json /home/mpukgame/server.json

ENTRYPOINT ["/game/duelo-server-0.0.0.0"]