#!/bin/sh
printenv
chmod a+x "unpack.sh"
chmod a+x "fastlane.sh"
chmod a+x "resign.sh"
chmod a+x "pack.sh"
./unpack.sh Versions.iOS $BundleIdentifier
./fastlane.sh
./resign.sh Versions.iOS $Provisioning
#./pack.sh