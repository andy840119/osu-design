#!/bin/bash
# This script was used to automatically convert vscode-icons SVG files to PNG using rsvg-convert.

for filename in ./*.svg; do
    for ((i=0; i<=3; i++)); do
        rsvg-convert -h 128 "$filename" > "$(basename "$filename" .svg).png"
    done
done