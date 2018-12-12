#! /bin/bash
# Builds wye and installs it
set -e

readonly root=`dirname "$0"`

nuget restore
msbuild -p:Configuration=Release

rm -rf /opt/wye
cp -r $root/Wye/bin/Release /opt/wye
mv /opt/wye/Wye.exe /opt/wye/wye
chmod a+x /opt/wye/wye

./add_path.sh

