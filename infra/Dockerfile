FROM unitymultiplay/linux-base-image:latest

WORKDIR /game
COPY --chown=mpukgame ./server_linux/ /game/

ENTRYPOINT ["/game/duelo-server-0.0.0.0"]