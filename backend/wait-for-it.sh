#!/bin/bash

# wait-for-it.sh

# Wait for a service to be available
# Usage: ./wait-for-it.sh <host>:<port> [-- <command>]
# Example: ./wait-for-it.sh db:5432 -- echo "DB is up"

host="$1"
shift
port="$1"
shift
cmd="$@"

until nc -z "$host" "$port"; do
  echo "Waiting for $host:$port to be available..."
  sleep 1
done

echo "$host:$port is up - executing command"
exec $cmd
