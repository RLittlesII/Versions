#!/bin/sh
bundle=$(wc -l < $1)
profile=$(wc -l < $2)
echo $bundle
echo $profile

printenv

chmod a+x "unpack.sh"
chmod a+x "fastlane.sh"
chmod a+x "resign.sh"
chmod a+x "pack.sh"

./unpack.sh "Versions.iOS" "$bundle"
./fastlane.sh
./resign.sh "Versions.iOS" "$profile"
#./pack.sh