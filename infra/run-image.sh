docker run --rm --platform linux/amd64 -it \
    --name duelo-server-instance \
    duelo-server:latest -- \
    -batchmode -nographics \
    --matchId TEST_MATCH_1
