#! /bin/bash
# Adds wye to the path variable if not already present
set -e

readonly export='export PATH=$PATH:/opt/wye'

if ! grep -q "$export" ~/.bashrc; then
    echo -e "\n$export" >> ~/.bashrc
fi
