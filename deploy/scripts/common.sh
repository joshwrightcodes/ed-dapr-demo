# standard output may be used as a return value in the functions
# we need a way to write text on the screen in the functions so that
# it won't interfere with the return value.
# Exposing stream 3 as a pipe to standard output of the script itself
exec 3>&1

if [ -t 1 ] && command -v tput > /dev/null; then
    # see if it supports colors
    ncolors=$(tput colors || echo 0)
    if [ -n "$ncolors" ] && [ $ncolors -ge 8 ]; then
        bold="$(tput bold       || echo)"
        normal="$(tput sgr0     || echo)"
        black="$(tput setaf 0   || echo)"
        red="$(tput setaf 1     || echo)"
        green="$(tput setaf 2   || echo)"
        yellow="$(tput setaf 3  || echo)"
        blue="$(tput setaf 4    || echo)"
        magenta="$(tput setaf 5 || echo)"
        cyan="$(tput setaf 6    || echo)"
        white="$(tput setaf 7   || echo)"
    fi
fi

################################################################################
# Writes a information message to STDOUT
# GLOBALS:
#  $SCRIPT_NAME: (Optional) Name of the script running
# ARGUMENTS:
#   $1: Message
# OUTPUTS:
#   Logs messages to STDOUT
################################################################################
log_inf() {
    # using stream 3 (defined in the beginning) to not interfere with stdout of functions
    # which may be used as return value
    printf "%b\n" "${magenta:-}$(date +"%Y-%m-%dT%H:%M:%S%z") ${green:-}${SCRIPT_NAME}: ${cyan:-}$1${normal:-}" >&3
}

################################################################################
# Writes a yellow error message to STDOUT
# GLOBALS:
#  $SCRIPT_NAME: (Optional) Name of the script running
# ARGUMENTS:
#   $1: Message
# OUTPUTS:
#   Logs messages to STDOUT
################################################################################
log_warning() {
    printf "%b\n" "${yellow:-}$(date +"%Y-%m-%dT%H:%M:%S%z") ${SCRIPT_NAME:-}: Warning: $1${normal:-}" >&3
}

################################################################################
# Writes a red error message to STDOUT
# GLOBALS:
#  $SCRIPT_NAME: (Optional) Name of the script running
# ARGUMENTS:
#   $1: Message
# OUTPUTS:
#   Logs messages to STDOUT
################################################################################
log_err() {
    printf "%b\n" "${red:-}$(date +"%Y-%m-%dT%H:%M:%S%z") ${SCRIPT_NAME:-}: Error: $1${normal:-}" >&2
}

# Exit Early if Bash is older than v4
if [[ "${BASH_VERSINFO:-0}" -lt 4 ]]; then
  log_err "This script requires bash >= 4, you are using \"${BASH_VERSION}\""
  exit 1
fi

################################################################################
# Builds, tags and pushes Docker image
# ARGUMENTS:
#   $1: Dockerfile path
#   $2: Docker Context Dir
#   $3: Docker Repo
#   $4: Docker Image Name
#   $5: Helm Chart Version
#   $6: Application Version
#   $7: Build
# OUTPUTS:
#   Logs messages to STDOUT
################################################################################
build_docker_image() {
  local dockerFile="${1}"
  local dockerContext="${2}"
  local dockerRepo="${3,,}"
  local dockerImageName="${4,,}"
  local chartVersion="${5}"
  local appVersion="${6}"
  local buildVersion="${7}"
  
  if [[ -n $dockerRepo ]]; then dockerRepo+="/"; fi
  
  log_inf "Building Docker Image: ${dockerRepo:-}${dockerImageName}"
  
  docker build --file "${dockerFile}" \
    --tag "${dockerRepo:-}${dockerImageName}:latest" \
    --tag "${dockerRepo:-}${dockerImageName}:${chartVersion}.${appVersion}" \
    --tag "${dockerRepo:-}${dockerImageName}:${chartVersion}.${appVersion}.${buildVersion}" \
    --build-arg "APP_VERSION=${chartVersion}.${appVersion}.${buildVersion}" \
    "${dockerContext}"

  log_inf "Built Docker Image: ${dockerRepo:-}${dockerImageName}:${chartVersion}.${appVersion}.${buildVersion}"
}

################################################################################
# Updates Helm Dependencies (if any) and rebuilds chart
# ARGUMENTS:
#   $1: Helm Chart Name
#   $2: Helm Chart Directory
# OUTPUTS:
#   Logs messages to STDOUT
################################################################################
update_helm_dependencies() {
  local chartName="${1}"
  local helmChart="${2}"
  
  log_inf "Updating Dependencies for \"${chartName}\""

  helm dependency update "${helmChart}"
  helm dependency build "${helmChart}"
}

################################################################################
# Updates the chart and app version in a given Helm Chart
# ARGUMENTS:
#   $1: Helm Chart Name
#   $2: Helm Chart File Path
#   $3: Helm Chart Version (must follow SemVer2)
#   $4: App Version
# OUTPUTS:
#   Logs messages to STDOUT
################################################################################
update_helm_version() {
  local chartName="${1}"
  local helmChart="${2}"
  local chartVersion="${3}"
  local appVersion="${4}"
  
  log_inf "Updating Chart Versions for ${chartName}"
  
  sed -i '' "s/^appVersion:.*$/appVersion: \"${appVersion}\"/" "${helmChart}/Chart.yaml"
  sed -i '' "s/^version:.*$/version: \"${chartVersion}\"/" "${helmChart}/Chart.yaml"
  
  log_inf "Updated ${chartName} to $chartVersion (${appVersion})"
}
