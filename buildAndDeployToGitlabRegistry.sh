#!/bin/bash
docker-compose build
docker login -u gitlab+deploy-token-398047 -p jtJZgUu3ynZMTwLrRhVx registry.gitlab.com/team5capstonegroup/team5-backend-dotnet-5
docker-compose push 
