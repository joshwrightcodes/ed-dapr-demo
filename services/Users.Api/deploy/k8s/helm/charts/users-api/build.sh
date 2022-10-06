#!/usr/bin/env bash

################################################################################
# Updates the chart and app version in a given Helm Chart
# GLOBALS:
#   $SCRIPT_NAME
#   $SCRIPT_ROOT
# ARGUMENTS:
#   $1: Helm Chart Version (must follow SemVer2)
#   $2: Application Version
#   $3: Application Build Version
#   $4: Docker Repo Name
#   $5: Docker Context
# OUTPUTS:
#   Logs messages to STDOUT
#   Pushes image to Docker Repo
################################################################################
set -euo pipefail
pushd "${0%/*}" || exit

CHART_VERSION="${1:-"1.0.0"}"
APP_VERSION="${2:-$(git rev-parse HEAD | cut -c 1-7)}"
BUILD_VERSION="${3:-$(date +"%s")}"
DOCKER_REPO="${4:-}"
DOCKER_CONTEXT=${5:-"../../../../../../.."}
DOCKER_FILE="../../../../../Dockerfile"
CHART_NAME="${PWD##*/}"
CHART_DIR=${PWD}
SCRIPT_NAME="${SCRIPT_NAME+=" > ":-} ${0##*/} (${CHART_NAME})"

. "${SCRIPT_ROOT:-"../../../../../../.."}/deploy/scripts/common.sh"

log_inf "Building ${CHART_NAME} (${CHART_VERSION}.${APP_VERSION}.${BUILD_VERSION})"
build_docker_image "${DOCKER_FILE}" "${DOCKER_CONTEXT}" "${DOCKER_REPO}" "${CHART_NAME}" "${CHART_VERSION}" "${APP_VERSION}" "${BUILD_VERSION}"
update_helm_version "${CHART_NAME}" "${CHART_DIR}" "${CHART_VERSION}" "${CHART_VERSION}.${APP_VERSION}.${BUILD_VERSION}"
update_helm_dependencies "${CHART_NAME}" "${CHART_DIR}"
log_inf "Built ${CHART_NAME} (${CHART_VERSION}.${APP_VERSION}.${BUILD_VERSION})"

popd || exit