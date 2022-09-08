#!/bin/bash

NAMESPACE="eddemo-dapr"
CMD=${1}

init() {
  echo "Deploying Kubernetes Certificate Manager"
  echo "----------"
  kubectl apply -f https://github.com/cert-manager/cert-manager/releases/latest/download/cert-manager.yaml
  cmctl check api --wait=2m
  echo "----------"
  
  echo "Deploying OpenTelemetry Operator"
  echo "----------"
  kubectl apply -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml
  kubectl wait --for=condition=available --timeout=120s deployment/opentelemetry-operator-controller-manager -n opentelemetry-operator-system
  echo "----------"
  
  echo "Initialised."
}

term() {
  # Create OpenTelemetry Operator
  echo "Terminating OpenTelemetry Operator"
  echo "----------"
  kubectl delete -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml
  kubectl wait --all --namespace opentelemetry-operator-system --timeout=120s --for=delete "pod"
  echo "----------"
    
  # Create Kubernetes Certificate Manager
  echo "Terminating Kubernetes Certificate Manager"
  echo "----------"
  kubectl delete -f https://github.com/cert-manager/cert-manager/releases/latest/download/cert-manager.yaml
  kubectl wait --all --namespace cert-manager --timeout=120s --for=delete "pod"
  echo "----------"
  
  echo "Terminated."
}

up() {
  # Create Namespace
  kubectl apply -f Infra/namespace.yaml
  
  # Create OpenTelemetry Collector
  kubectl apply -f Infra/otel-collector.yaml -n eddemo-dapr
  kubectl wait --for=condition=available --timeout=120s deployment/otelcol-collector -n eddemo-dapr
  
  # Create User Groups Demo Service
  kubectl apply -f Services/UserGroups.Api/UserGroups.Api.yaml  -n eddemo-dapr

}

down() {
  # Tear Down User Groups Demo Service
  echo "Tearing down User Group Api"
  kubectl delete -f Services/UserGroups.Api/UserGroups.Api.yaml -n ${NAMESPACE}
  kubectl wait deployment/usergroups-api --for=delete -n ${NAMESPACE}
  
  # Tear Down OpenTelemetry Collector
  echo "Tearing down OpenTelemetry Collector"
  kubectl delete -f Infra/otel-collector.yaml -n ${NAMESPACE}
  kubectl wait deployment/otelcol-collector --for=delete -n ${NAMESPACE}
  
  # Tear Down Namespace
  echo "Tearing down Namespace"
  kubectl delete -f Infra/namespace.yaml
  kubectl wait namespace/${NAMESPACE} --for=delete
}

case ${CMD} in
  init)
    init
    ;;
  term)
    term
    ;;
  up)
    up
    ;;
  down)
    down
    ;;
  *)
    echo "Not a valid option. Valid options are 'init', 'term', 'up', 'down'"
    ;;
esac