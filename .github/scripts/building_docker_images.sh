#!/bin/bash

$ timestamp=$(date +%Y%m%d%H%M%S)  
echo "Start building images. Time: $timestamp"

cd ../../

docker-compose build

$ elapsedTime=$(date +%Y%m%d%H%M%S)-timestamp
echo "End building images. Elapsed time: $elapsedTime"

docker system prune -f
