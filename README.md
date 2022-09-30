# Dapr Demo

Demo using Kubernetes and Dapr for Microservices

## Prerequisites

The following software is required to be installed to run this application

1. Golang
2. Kubernetes _flavour of your choosing_
   - if using minikube with docker engine, run the following if you have issues finding the image
   ```shell
   eval $(minikube docker-env)
   ```
   
3. Kubernetes Command Line Tool (`kubectl`)
4. Kubernetes cert-manager Command Line Tool (`cmctl`)
   This can be installed by running the below script in `bash` or similar shell:
   ```shell
   OS=$(go env GOOS); ARCH=$(go env GOARCH); curl -sSL -o cmctl.tar.gz https://github.com/cert-manager/cert-manager/releases/download/v1.7.2/cmctl-$OS-$ARCH.tar.gz
   tar xzf cmctl.tar.gz
   sudo mv cmctl /usr/local/bin
   rm cmctl.tar.gz
   rm -rf cmctl.tar.gz
   ```
5. Helm Package Manager  
   This can be installed from a package manager of choice (eg: Homebrew `brew install dapr`)
   or by running the below script
   ```shell
   curl -fsSL -o get_helm.sh https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3
   chmod 700 get_helm.sh
   ./get_helm.sh
   ```
6. Dapr CLI
   This can be installed from a package manager of choice (eg: Homebrew `brew install dapr/tap/dapr-cli`)
   or by running the below script:
   ```shell
   curl -fsSL https://raw.githubusercontent.com/dapr/cli/master/install/install.sh | /bin/bash
   ```
   [Read more here](https://docs.dapr.io/getting-started/install-dapr-cli/)


## Getting Started

### Prepare Environment

This project has the following requirements that need to be installed prior to running the solution:
- Certificate Manager
- Dapr (CLI & Runtime)
- NGINX Ingress Controller
- OpenTelemetry Operator

All commands are from the root directory.

1. Run `install.sh` - this will install all your first run requirements in Kubernetes

### Running the application

1. Run Deploy Script
   ```shell
   ./deploy.sh
   ```

### Removing the application

1. Uninstall Helm Chart
   ```shell
   helm uninstall dapr-demo \
     --namespace dapr-demo
   ```
2. Delete Namespace
   ```shell
   kubectl delete namespace/dapr-demo
   ```

## Further Reading
- [OpenTelemetry Collector - Getting Started](https://opentelemetry.io/docs/collector/getting-started/)
- [Dapr - Getting Started](https://docs.dapr.io/getting-started/)
- [Deploy Dapr on a Kubernetes cluster](https://docs.dapr.io/operations/hosting/kubernetes/kubernetes-deploy/)





