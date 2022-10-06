#! /usr/local/bin/bash
SCRIPT_NAME="install_dependencies"

source deploy/scripts/common.sh
say "Install Dependencies for ${APP_NAME}"

##### DECLARE DEPENDENCIES HERE
declare -A dependency0=(
  [Name]="Metrics Server"
  [Chart]="metrics-server"
  [Namespace]="metrics-server-system"
  [Repo]="https://kubernetes-sigs.github.io/metrics-server/"
)

declare -A dependency1=(
  [Name]="Metal Load Balancer"
  [Chart]="metallb"
  [Namespace]="metallb-system"
  [Repo]="https://metallb.github.io/metallb"
)

declare -A dependency2=(
  [Name]="Certificate Manager"
  [Chart]="cert-manager"
  [Namespace]="cert-manager-system"
  [Repo]="https://charts.jetstack.io"
)

declare -A dependency3=(
  [Name]="Dapr"
  [Chart]="dapr"
  [Namespace]="dapr-system"
  [Repo]="https://dapr.github.io/helm-charts"
)
  
declare -A dependency4=(
  [Name]="NGINX Ingress"
  [Chart]="ingress-nginx"
  [Namespace]="ingress-nginx-system"
  [Repo]="https://kubernetes.github.io/ingress-nginx"
)
  
declare -A dependency5=(
  [Name]="OpenTelemetry Operator"
  [Chart]="opentelemetry-operator"
  [Namespace]="opentelemetry-operator-system"
  [Repo]="https://open-telemetry.github.io/opentelemetry-helm-charts"
)
#####
declare -n dependency
##### 

# Args
# $1 Name
# $2 Chart
# $3 Repo
# $4 Namespace
install_dependency() {
  log_inf "Install ${bold:-}${1}${normal:-}"
  
  if test -f "${DEPENDENCIES_DIR}/values/${2}.values.yaml"; then
      helm upgrade \
        --install "${2}" "${2}" \
        --repo "${3}" \
        --namespace "${4}" --create-namespace \
        --values "${DEPENDENCIES_DIR}/values/${2}.values.yaml" \
        --wait --timeout="${HELM_TIMEOUT}"
  else
    log_warning "No values file for chart ${2}, using chart defaults"
    
    helm upgrade \
      --install "${2}" "${2}" \
      --repo "${3}" \
      --namespace "${4}" --create-namespace \
      --wait --timeout="${HELM_TIMEOUT}"
  fi
  
  shopt -s nullglob

  for bootstrap in ${DEPENDENCIES_DIR}/bootstrap/${2}*.yaml; do
    log_inf "Applying config ${bootstrap}"
    kubectl apply -f "${bootstrap}"
  done

  shopt -u nullglob
}

DEPENDENCIES_DIR="deploy/k8s/helm/dependencies"
log_inf "Dependencies Directory: ${DEPENDENCIES_DIR}"
HELM_TIMEOUT="2m"
log_inf "Helm Timeout: ${HELM_TIMEOUT}"

log_inf "Deploying Dependencies"

for dependency in ${!dependency@}; do
  install_dependency "${dependency[Name]}" "${dependency[Chart]}" "${dependency[Repo]}" "${dependency[Namespace]}"
done

log_inf "Finished Deploying Dependencies for ${APP_NAME}"
log_inf "Completed in:"
times
exit 0