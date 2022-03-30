#!/bin/sh
chmod a+x "unpack.sh"
"$System.DefaultWorkingDirectory"/_RLittlesII.Versions/drop/unpack.sh Versions.iOS "$BundleIdentifier"
#./fastlane.sh
#./resign.sh Versions.iOS "$Provisioning"
#./pack.sh