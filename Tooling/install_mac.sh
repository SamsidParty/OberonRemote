#!/bin/zsh

cd ~/Downloads
curl -OL https://github.com/SamsidParty/OberonRemote/releases/latest/download/Oberon.Remote.Mac.zip

unzip "./Oberon.Remote.Mac.zip" -d "./"
chmod +x "./Oberon Mac.app/Contents/MacOS/Oberon Remote"
xattr -d com.apple.quarantine "./Oberon Mac.app" 
mv "./Oberon Mac.app" "/Applications/Oberon Mac.app" 
rm -rf "./Oberon.Remote.Mac.zip"

open -a "/Applications/Oberon Mac.app" 