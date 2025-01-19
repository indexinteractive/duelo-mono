VERSION=0.0.2
MULTIPLAY_TAG="registry.multiplay.com/1e416a51-76cd-4851-9d30-7a9ad4c6ec80/28b45ed6-2900-4d83-bcff-426e2d1ef020/116082:$VERSION"
UGS_USER_KEY="af1044f5-3d58-485e-9c40-7a1f67df1868"
UGS_USER_SECRET="3GxlfW-CmOpRWxPu-8B7Gl589MuKdrPh"

docker login -u $UGS_USER_KEY -p $UGS_USER_SECRET registry.multiplay.com

docker tag duelo-server:latest $MULTIPLAY_TAG
docker push $MULTIPLAY_TAG