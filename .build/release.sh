printenv
./unpack.sh Versions.iOS "$BundleIdentifier"
./fastlane.sh
#./resign.sh Versions.iOS "$Provisioning"
#./pack.sh