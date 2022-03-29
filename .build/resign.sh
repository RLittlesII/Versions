n1=$(wc -l < $1)
n2=$(wc -l < $2)
codesign -d --entitlements :entitlements.plist /Payload/$1.app
codesign -f -s '$2' --entitlements entitlements.plist