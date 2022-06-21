#!/bin/bash

timestamp= $(date)
echo Start building images. Time: $timestamp

docker-compose build --force-rm

newTimestamp = $(date)
echo End building images. Elapsed time: $(newTimestamp - timestamp)

docker system prune -f
