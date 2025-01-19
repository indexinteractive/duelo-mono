SCRIPT_DIR=`(pwd)`
UNITY_PATH="$HOME/Unity/Hub/Editor/6000.1.0a9/Editor/Unity"
PROJECT_PATH="$HOME/ind3x/duelo-mono/duelo-unity"
BUILDS_ROOT="$PROJECT_PATH/Builds"
APP_NAME=duelo-server

echo ""
echo "DUELO build script"
echo "------------------"
echo ""
echo "App name: $APP_NAME"
echo "Unity path: $UNITY_PATH"
echo "Project path: $PROJECT_PATH"
echo "Build output path: $BUILDS_ROOT"
echo ""

echo "Cleaning out existing build files"
rm -rf $BUILDS_ROOT/**/*

echo "Running Unity script"

$UNITY_PATH \
    -batchmode \
    -nographics \
    -quit \
    -projectPath "$PROJECT_PATH" \
    -executeMethod Duelo.Build.BuildScript.BuildServer \
    -logFile "build.log" \
    --buildFolder "$BUILDS_ROOT/server_standalone" \
    --appName "$APP_NAME"

if grep -q "Build Finished, Result: Success." build.log; then
    echo "Unity build finished successfully"
    echo "Logs: $(pwd)/build.log"
    echo "Builds: $PROJECT_PATH/Builds"

    VERSION=$(grep -oP '\[BuildScript\] Version: \K[0-9]+\.[0-9]+\.[0-9]+' build.log)
    echo "Build version: $VERSION"
else
    echo "Build failed. Check the log file for details:"
    echo "$(pwd)/build.log"
    exit 1
fi

cp Dockerfile $BUILDS_ROOT
cp server.json $BUILDS_ROOT
cd $BUILDS_ROOT

DOCKER_TAG="$APP_NAME:$VERSION"

echo "Building Docker image: $DOCKER_TAG"
docker build --build-arg VERSION=$VERSION -t $DOCKER_TAG .

cd $SCRIPT_DIR;
./push-image.sh $VERSION