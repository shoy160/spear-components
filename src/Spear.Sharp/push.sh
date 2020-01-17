#!/bin/sh
version=$1
image=docker.dev:5000/spear:$version
docker build -t $image . && docker push $image && docker rmi $image