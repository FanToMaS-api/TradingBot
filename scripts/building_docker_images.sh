#!/bin/bash

timestamp= $(date -d)
echo Start building images. Time: $(timestamp)

docker-compose build --force-rm

newTimestamp = $(date -d)
echo End building images. Elapsed time: $(newTimestamp - timestamp)

docker system prune -f
