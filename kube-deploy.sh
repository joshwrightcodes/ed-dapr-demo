#!/usr/bin/env bash
set -e
export DEMOCLI_WORKDIR=$(cd "$(dirname "${0}")" && pwd)
. "$DEMOCLI_WORKDIR/scripts/common"

NAMESPACE="eddemo-dapr"
CMD=${1}

cli_help() {
  cli_name=${0##*/}
  echo "
$cli_name
Dapr Demo CLI
Version: $(cat "${DEMOCLI_WORKDIR}/scripts/VERSION")
Usage: $cli_name [command]
Commands:
  init|i    Installs dependencies
  term|t    Uninstalls dependencies
  up|u      Deploys/Updates services
  down|d    Tears down services
  *         Help
"
  exit 1
}

init() {
  cli_log "Deploying Kubernetes Certificate Manager"
  kubectl apply -f https://github.com/cert-manager/cert-manager/releases/latest/download/cert-manager.yaml
  cmctl check api --wait=2m
  
  cli_log "Deploying Ingress Controller"
  helm upgrade --install ingress-nginx ingress-nginx \
    --repo https://kubernetes.github.io/ingress-nginx \
    --namespace ingress-nginx --create-namespace
  
  kubectl wait --namespace ingress-nginx \
    --for=condition=ready pod \
    --selector=app.kubernetes.io/component=controller \
    --timeout=120s
  
  cli_log "Deploying OpenTelemetry Operator"
  kubectl apply -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml
  kubectl wait --for=condition=available --timeout=120s deployment/opentelemetry-operator-controller-manager -n opentelemetry-operator-system
  
  cli_log "Initialised."
}

term() {
  # Create OpenTelemetry Operator
  cli_log "Terminating OpenTelemetry Operator"
  kubectl delete -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml
  kubectl wait --all --namespace opentelemetry-operator-system --timeout=120s --for=delete "pod"
  
  cli_log "Terminating Ingress Controller"
  kubectl delete -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.3.1/deploy/static/provider/cloud/deploy.yaml
  kubectl wait -n ingress-nginx --timeout=120s --all --for=delete "pod"
    
  # Create Kubernetes Certificate Manager
  cli_log "Terminating Kubernetes Certificate Manager"
  kubectl delete -f https://github.com/cert-manager/cert-manager/releases/latest/download/cert-manager.yaml
  kubectl wait --all --namespace cert-manager --timeout=120s --for=delete "pod"
  
  cli_log "Terminated."
}

up() {
  # Create Namespace
  kubectl apply -f Infra/namespace.yaml
  
  # Create OpenTelemetry Collector
  kubectl apply -f Infra/otel-collector.yaml -n ${NAMESPACE}
  kubectl wait --for=condition=available --timeout=120s deployment/otelcol-collector -n ${NAMESPACE}
  
  # Create User Groups Demo Service
  kubectl apply -f Services/UserGroups.Api/UserGroups.Api.yaml  -n ${NAMESPACE}
}

down() {
  # Tear Down User Groups Demo Service
  cli_log "Tearing down User Group Api"
  kubectl delete -f Services/UserGroups.Api/UserGroups.Api.yaml -n ${NAMESPACE}
  kubectl wait deployment/usergroups-api --for=delete -n ${NAMESPACE}
  
  # Tear Down OpenTelemetry Collector
  cli_log "Tearing down OpenTelemetry Collector"
  kubectl delete -f Infra/otel-collector.yaml -n ${NAMESPACE}
  kubectl wait deployment/otelcol-collector --for=delete -n ${NAMESPACE}
  
  # Tear Down Namespace
  cli_log "Tearing down Namespace"
  kubectl delete -f Infra/namespace.yaml
  kubectl wait namespace/${NAMESPACE} --for=delete
}

case ${CMD} in
  init|i)
    init
    ;;
  term|t)
    term
    ;;
  up|u)
    up
    ;;
  down|d)
    down
    ;;
  *)
    cli_help
    ;;
esac