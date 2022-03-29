n1=$(wc -l < $1)
n2=$(wc -l < $2)
echo $n1
echo $n2

ipa="$1.ipa"
echo $ipa
zip="$1.zip"
echo $zip
appName="$1.app"
cp ../artifacts/ios/"$ipa" ../artifacts/ios/"$zip"

unzip -d ../artifacts/ios/"$1" ../artifacts/ios/"$zip"

echo ../artifacts/ios/Payload/"$appName"

/usr/libexec/PlistBuddy -c "Set :CFBundleIdentifier $2" ../artifacts/ios/"$1"/Payload/"$appName"/info.plist

rm -rf ../artifacts/ios/"$1"/Payload/"$appName"/_CodeSignature