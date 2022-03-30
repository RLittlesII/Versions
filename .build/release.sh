#!/bin/sh
chmod a+x "unpack.sh"
"$SYSTEM_DEFAULTWORKINGDIRECTORY"/unpack.sh Versions.iOS "$BundleIdentifier"
#./fastlane.sh
#./resign.sh Versions.iOS "$Provisioning"
#./pack.sh