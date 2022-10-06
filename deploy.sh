#! /usr/local/bin/bash
set -euo pipefail

APP_NAME="Dapr Demo"
export APP_NAME

SCRIPT_NAME="deploy_app"
export SCRIPT_NAME

SCRIPT_ROOT="$(pwd)"
export SCRIPT_ROOT

source deploy/scripts/common.sh

log_inf "Bash version: ${BASH_VERSION}"
log_inf "Deploying ${APP_NAME}"

DOCKER_REPO="daprdemo"
CHART_VERSION=0.1.0
HASH=$(git rev-parse HEAD | cut -c 1-7)
BUILD="$(date +"%s")"

log_inf "Building Services"
for dockerfile in services/**/Dockerfile; do
  PROJECT_DIR=${dockerfile%/*}
  BUILD_SCRIPT="$(find "${PROJECT_DIR}" -name "build.sh")"
  if [ -n "$BUILD_SCRIPT" ]; then
    eval "${BUILD_SCRIPT}" "${CHART_VERSION}" "${HASH}" "${BUILD}" "${DOCKER_REPO}" "${SCRIPT_ROOT}"
  fi
done
log_inf "Services Built"

log_inf "Preparing App"

update_helm_version "dapr-demo" "deploy/k8s/helm/charts/dapr-demo" "${CHART_VERSION}" "${CHART_VERSION}.${HASH}.${BUILD}"
update_helm_dependencies "dapr-demo" "deploy/k8s/helm/charts/dapr-demo"

log_inf "Deploying ${APP_NAME}"

helm upgrade --install dapr-demo deploy/k8s/helm/charts/dapr-demo/ \
  --namespace dapr-demo --create-namespace \
  --wait --timeout=30s
  
log_inf "Deployed ${APP_NAME}"
times