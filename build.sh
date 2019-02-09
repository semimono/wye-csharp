#! /bin/bash
# Builds wye
set -e

readonly root=`dirname "$0"`

cd $root

msbuild