#!/bin/bash

# Prompt user for the version number
read -p "Enter the version (e.g., 1.0.0): " VERSION

# Check if the user entered a version
if [[ -z "$VERSION" ]]; then
    echo "Error: Version cannot be empty."
    exit 1
fi

echo "You entered version: $VERSION"
echo "Starting upload process..."

# Define platforms and directory mappings
declare -A platforms=(
    ["Linux"]="Linux"
    ["Android"]="Android"
    ["Windows"]="Windows"
)

# Temporary directory for excluded folders
TEMP_DIR=$(mktemp -d)
cleanup() {
    echo "Restoring folders..."
    for platform in "${!platforms[@]}"; do
        directory=${platforms[$platform]}
        if [[ -d "$TEMP_DIR/$directory" ]]; then
            mv "$TEMP_DIR/$directory"/* "$directory/" 2>/dev/null
        fi
    done
    rm -rf "$TEMP_DIR"
    echo "Cleanup completed."
}
trap cleanup EXIT

# Prepare and push builds for each platform
for platform in "${!platforms[@]}"; do
    directory=${platforms[$platform]}
    lowercase_platform=$(echo "$platform" | tr '[:upper:]' '[:lower:]')

    # Move folders ending with _DoNotShip to the temp directory
    echo "Preparing $platform build..."
    mkdir -p "$TEMP_DIR/$directory"
    find "$directory" -type d -name '*_DoNotShip' -exec mv {} "$TEMP_DIR/$directory/" \;

    echo "Pushing $platform build from directory '$directory'..."
    butler push "$directory" "nicholastoya/cinetica:$lowercase_platform" --userversion "$VERSION"

    # Check if the last command succeeded
    if [[ $? -ne 0 ]]; then
        echo "Error: Failed to push $platform build."
        exit 1
    fi
done

echo "All builds pushed successfully with version $VERSION!"
