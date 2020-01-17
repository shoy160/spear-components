#!/bin/bash
version=$1
image="docker.dev:5000/spear:$version"
echo $image
docker build -t ${image} . && docker push ${image} && docker rmi ${image}