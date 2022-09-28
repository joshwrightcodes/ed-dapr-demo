#! /usr/local/bin/bash

set -e
echo "$BASH_VERSION"

CHART_VERSION=0.1.0
VERSION_SUFFIX=$(git rev-parse HEAD | cut -c 1-7)
VERSION_INSTANCE="$(date +"%s")"
ASSEMBLY_INFO_VERSION="${CHART_VERSION}.${VERSION_SUFFIX}_${VERSION_INSTANCE}"

echo "$(date +"%Y-%m-%dT%H:%M:%S%z") --- Version: ${ASSEMBLY_INFO_VERSION}"

echo "$(date +"%Y-%m-%dT%H:%M:%S%z") --- Building Docker Images"

DOCKER_REPO=daprdemo

for filename in ./services/*/Dockerfile; do
  echo "$(date +"%Y-%m-%dT%H:%M:%S%z") ----- Building: \"${filename}\""
  DOCKER_DIR="${filename%/*}"
  IMAGE_NAME=$(basename "${DOCKER_DIR,,}")
  docker build --file "${filename}" \
    --tag "${DOCKER_REPO}/${IMAGE_NAME}:latest" \
    --tag "${DOCKER_REPO}/${IMAGE_NAME}:${CHART_VERSION}.${VERSION_SUFFIX}" \
    --tag "${DOCKER_REPO}/${IMAGE_NAME}:${ASSEMBLY_INFO_VERSION}" \
    --build-arg "VERSION=${CHART_VERSION}" \
    --build-arg "VERSION_SUFFIX=${VERSION_SUFFIX}" \
    --build-arg "ASSEMBLY_INFO_VERSION=${ASSEMBLY_INFO_VERSION}" \
    .
done

echo "$(date +"%Y-%m-%dT%H:%M:%S%z") --- Updating Chart App Versions to ${ASSEMBLY_INFO_VERSION}"

for filename in ./deploy/k8s/helm/*/Chart.yaml; do
  CHART_DIR="${filename%/*}"
  echo "$(date +"%Y-%m-%dT%H:%M:%S%z") ----- Updating: \"$(basename "${CHART_DIR,,}")\""
  sed -i '' "s/^appVersion:.*$/appVersion: \"${ASSEMBLY_INFO_VERSION}\"/" "${filename}"
  sed -i '' "s/^version:.*$/version: \"${CHART_VERSION}\"/" "${filename}"
done

echo "$(date +"%Y-%m-%dT%H:%M:%S%z") --- Building Chart Dependencies"

for filename in ./deploy/k8s/helm/*/Chart.yaml; do
  CHART_DIR="${filename%/*}"
  echo "$(date +"%Y-%m-%dT%H:%M:%S%z") ----- Building: \"$(basename "${CHART_DIR,,}")\""
  helm dependency update "${CHART_DIR}"
  helm dependency build "${CHART_DIR}"
done

echo "$(date +"%Y-%m-%dT%H:%M:%S%z") --- Deploying Chart"

helm upgrade --install dapr-demo ./deploy/k8s/helm/z-dapr-demo/ \
  --namespace dapr-demo --create-namespace \
  --wait --timeout=240s
  
echo "$(date +"%Y-%m-%dT%H:%M:%S%z") --- Done."