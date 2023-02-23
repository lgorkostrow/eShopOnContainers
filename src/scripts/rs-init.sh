#!/bin/bash

DELAY=5

mongosh <<EOF
var config = {
    "_id": "dbrs",
    "version": 1,
    "members": [
        {
            "_id": 1,
            "host": "nosqldata:27017",
            "priority": 2
        },
        {
            "_id": 2,
            "host": "nosqldata2:27017",
            "priority": 1
        },
        {
            "_id": 3,
            "host": "nosqldata3:27017",
            "priority": 1
        }
    ]
};
rs.initiate(config, { force: true });
EOF

echo "****** Waiting for ${DELAY} seconds for replicaset configuration to be applied ******"

sleep $DELAY

mongosh < /scripts/init.js