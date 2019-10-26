#!/bin/sh
psql -c "\copy csv FROM '/tmp/source.csv' delimiter ',' csv;"