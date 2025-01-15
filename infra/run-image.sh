docker run --rm -it \
    --name duelo-server-instance \
    duelo-server:latest -- \
    -batchmode -nographics \
    --expire 10 \
    --matchId TEST_MATCH_1
