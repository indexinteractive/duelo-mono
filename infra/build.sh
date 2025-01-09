UNITY_PATH=/Applications/Unity/Hub/Editor/6000.1.0a9/Unity.app/Contents/MacOS/Unity
PROJECT_PATH=/Users/mremo/ind3x/02_games/duelo/duelo-unity
BUILDS_ROOT=$PROJECT_PATH/Builds

echo "Building server from $PROJECT_PATH"


$UNITY_PATH \
    -batchmode \
    -nographics \
    -quit \
    -projectPath "$PROJECT_PATH" \
    -executeMethod Duelo.Build.BuildScript.BuildServer \
    -logFile "build.log" \
    --buildFolder "$BUILDS_ROOT/server_linux" \
    --appName "duelo-server" \

if [ $? -ne 0 ]; then
    echo "Build failed. Check the log file for details: $(pwd)/build.log"
else
    echo "Build finished successfully"
    echo "Logs: $(pwd)/build.log"
    echo "Builds: $PROJECT_PATH/Builds"
fi

cp Dockerfile $BUILDS_ROOT
cd $BUILDS_ROOT

docker build --platform linux/amd64 -t duelo-server:latest .

# docker buildx create --use
# docker buildx build --platform linux/amd64 -t duelo-server:latest --load .