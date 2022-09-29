# Stop script on NZEC
set -e
# Stop script if unbound variable found (use ${var:-} if intentional)
set -u
# By default cmd1 | cmd2 returns exit code of cmd2 regardless of cmd1 success
# This is causing it to fail
set -o pipefail

# standard output may be used as a return value in the functions
# we need a way to write text on the screen in the functions so that
# it won't interfere with the return value.
# Exposing stream 3 as a pipe to standard output of the script itself
exec 3>&1

APP_NAME="Dapr Demo"

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

# Args
# $1 Message
say() {
    # using stream 3 (defined in the beginning) to not interfere with stdout of functions
    # which may be used as return value
    printf "%b\n" "${magenta:-}$(date +"%Y-%m-%dT%H:%M:%S%z") ${green:-}${SCRIPT_NAME}: ${cyan:-}$1${normal:-}" >&3
}

# Args
# $1 Message
say_warning() {
    printf "%b\n" "${yellow:-}$(date +"%Y-%m-%dT%H:%M:%S%z") ${SCRIPT_NAME}: Warning: $1${normal:-}" >&3
}

# Args
# $1 Message
say_err() {
    printf "%b\n" "${red:-}$(date +"%Y-%m-%dT%H:%M:%S%z") ${SCRIPT_NAME}: Error: $1${normal:-}" >&2
}

# Exit Early if Bash is older than v4
if [[ "${BASH_VERSINFO:-0}" -lt 4 ]]; then
  say_err "This script requires bash >= 4, you are using \"${BASH_VERSION}\""
  exit 1
fi

say "Bash version: ${BASH_VERSION}"