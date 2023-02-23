#!/bin/bash

DELAY=5

docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

echo "****** Waiting for ${DELAY} seconds for containers to go up ******"
sleep $DELAY

docker exec -it src_nosqldata /scripts/rs-init.sh