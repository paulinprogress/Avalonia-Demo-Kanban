#!/bin/bash

# Automated build and zipping script for Avalonia-Demo-Kanban
# Builds self-contained packages for all supported platforms and creates zip archives

set -e  # Exit on any error

PROJECT_NAME="Avalonia-Demo-Kanban"
CONFIGURATION="Release"
OUTPUT_DIR="bin/${CONFIGURATION}/net9.0"
RIDS=("win-x64" "win-x86" "linux-x64" "linux-arm64" "osx-x64" "osx-arm64")

echo "Starting automated build and packaging for ${PROJECT_NAME}..."

for RID in "${RIDS[@]}"; do
    echo "Building for ${RID}..."

    # Publish with self-contained=true
    dotnet publish -r "${RID}" -c "${CONFIGURATION}" --self-contained=true

    # Check if publish succeeded
    PUBLISH_DIR="${OUTPUT_DIR}/${RID}/publish"
    if [ ! -d "${PUBLISH_DIR}" ]; then
        echo "Error: Publish directory ${PUBLISH_DIR} not found!"
        exit 1
    fi

    # Create zip archive
    ZIP_NAME="${PROJECT_NAME}-${RID}.zip"
    echo "Creating ${ZIP_NAME}..."
    cd "${OUTPUT_DIR}"
    zip -r -j "${ZIP_NAME}" "${RID}/publish/"
    cd - > /dev/null

    echo "Completed ${RID}"
done

echo "All builds and zips completed successfully!"
echo "Archives created in ${OUTPUT_DIR}:"
ls -la "${OUTPUT_DIR}"/*.zip