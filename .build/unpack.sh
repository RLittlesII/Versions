n1=$(wc -l < $1)
n2=$(wc -l < $2)
echo $n1
echo $n2

ipa="$1.ipa"
echo $ipa
zip="$1.zip"
echo $zip
appName="$1.app"
cp "$ipa" "$zip"

unzip -d "$1" "$zip"

echo ./Payload/"$appName"

/usr/libexec/PlistBuddy -c "Set :CFBundleIdentifier $2" ./"$1"/Payload/"$appName"/info.plist

rm -rf ./"$1"/Payload/"$appName"/_CodeSignature