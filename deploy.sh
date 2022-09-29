#! /usr/local/bin/bash
SCRIPT_NAME="deploy_app"

source deploy/scripts/common.sh
say "Deploying ${APP_NAME}"

CHARTS_DIR="deploy/k8s/helm/charts/"
say "Charts Location: ${CHARTS_DIR}"
CHART_VERSION=0.1.0
VERSION_SUFFIX=$(git rev-parse HEAD | cut -c 1-7)
VERSION_INSTANCE="$(date +"%s")"
ASSEMBLY_INFO_VERSION="${CHART_VERSION}.${VERSION_SUFFIX}_${VERSION_INSTANCE}"
say "Chart Version: ${ASSEMBLY_INFO_VERSION}"

say "Building Docker Images"
DOCKER_REPO=daprdemo
say "Docker Repo: ${DOCKER_REPO}"

for filename in ./services/*/Dockerfile; do
  say "Building Docker Image: \"${filename}\""
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
    say "Built Docker Image: ${DOCKER_REPO}/${IMAGE_NAME}:${ASSEMBLY_INFO_VERSION}"
done

say "Updating Chart App Versions to ${ASSEMBLY_INFO_VERSION} and building dependencies"

for filename in ${CHARTS_DIR}*/Chart.yaml; do
  CHART_DIR="${filename%/*}"
  say "Updating version for \"$(basename "${CHART_DIR,,}")\""
  sed -i '' "s/^appVersion:.*$/appVersion: \"${ASSEMBLY_INFO_VERSION}\"/" "${filename}"
  sed -i '' "s/^version:.*$/version: \"${CHART_VERSION}\"/" "${filename}"
  
  say "Updating Dependencies for \"$(basename "${CHART_DIR,,}")\""
  helm dependency update "${CHART_DIR}"
  helm dependency build "${CHART_DIR}"
done

say "Deploying Main Application To K8s"

helm upgrade --install dapr-demo ${CHARTS_DIR}/z-dapr-demo/ \
  --namespace dapr-demo --create-namespace \
  --wait --timeout=240s
  
say "Deployed ${APP_NAME}"
say "Completed in:"
times
exit 0