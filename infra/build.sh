UNITY_PATH="$HOME/Unity/Hub/Editor/6000.1.0a9/Editor/Unity"
PROJECT_PATH="$HOME/ind3x/duelo-mono/duelo-unity"
BUILDS_ROOT="$PROJECT_PATH/Builds"

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
cp server.json $BUILDS_ROOT
cd $BUILDS_ROOT

docker build -t duelo-server:latest .