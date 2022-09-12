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
  helm upgrade \
    --install cert-manager cert-manager \
    --repo https://charts.jetstack.io \
    --namespace cert-manager --create-namespace \
    --set installCRDs=true \
    --wait --timeout=120s
  cmctl check api --wait=2m
  
  cli_log "Deploying Ingress Controller"
  helm upgrade \
    --install ingress-nginx ingress-nginx \
    --repo https://kubernetes.github.io/ingress-nginx \
    --namespace ingress-nginx --create-namespace \
    --wait --timeout=120s
      
  cli_log "Deploying Dapr"
  helm upgrade \
    --install dapr dapr \
    --repo https://dapr.github.io/helm-charts/ \
    --namespace dapr-system --create-namespace \
    --set global.ha.enabled=true \
    --wait --timeout=120s
    
  
  cli_log "Deploying OpenTelemetry Operator"
  helm upgrade --install opentelemetry-operator opentelemetry-operator \
    --repo https://open-telemetry.github.io/opentelemetry-helm-charts \
    --namespace opentelemetry-operator-system --create-namespace \
    --wait --timeout=120s
  
  cli_log "Initialised."
}

term() {
  # Create OpenTelemetry Operator
  cli_log "Removing OpenTelemetry Operator"
  helm uninstall opentelemetry-operator \
      --namespace opentelemetry-operator-system \
      --wait --timeout=120s

  cli_log "Removing Dapr"
  helm uninstall dapr \
    --namespace dapr-system \
    --wait --timeout=120s
  
  cli_log "Removing Ingress Controller"
  helm uninstall ingress-nginx \
    --namespace ingress-nginx \
    --wait --timeout=120s
  
  # Create Kubernetes Certificate Manager
  cli_log "Terminating Kubernetes Certificate Manager"
  helm uninstall cert-manager \
    --namespace cert-manager \
    --wait --timeout=120s
  
  cli_log "Terminated."
}

up() {
  local label=$(date +%s)
  # Create Namespace
  cli_log "Creating Namespace"
  kubectl apply -f Infra/namespace.yaml
  
  # Create Ingress
  cli_log "Creating Ingress"
  kubectl apply -f Infra/ingress.yaml
  
  # Create OpenTelemetry Collector
  cli_log "Creating OpenTelemetry Collector"
  kubectl apply -f Infra/otel-collector.yaml -n ${NAMESPACE}
  kubectl wait --for=condition=available --timeout=120s deployment/otel-collector -n ${NAMESPACE}
  
  # Create User Groups Demo Service
  cli_log "Building User Groups Api"
  docker build  \
    --tag "ed-demo/dapr-usergroups-api:latest" \
    --tag "ed-demo/dapr-usergroups-api:$label" \
    --file Services/UserGroups.Api/Dockerfile \
    .
  cli_log "Creating User Groups Api"
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
  kubectl wait deployment/otel-collector --for=delete -n ${NAMESPACE}
  
  # Tearing Down Ingress
  cli_log "Tearing down Ingress"
  kubectl delete -f Infra/ingress.yaml
  
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