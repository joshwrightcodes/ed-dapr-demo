# Dapr Demo

## Prerequisites

The following software is required to be installed to run this application

1. Golang
2. Kubernetes _flavour of your choosing_
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
   This can be installed from a package manager of choice (eg: Homebrew `brew install helm`)
   or by running the below script

   ```shell
   curl -fsSL -o get_helm.sh https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3
   chmod 700 get_helm.sh
   ./get_helm.sh
   ```

## Getting Started

1. (Optional) Fix script permissions
   ```shell
   chmod +x kube-deploy.sh
   ```
2. Run script `./kube-deploy.sh init` to install the initial requirements
   - Kubernetes Certificate Manager
   - NGINX Ingress Controller
   - OpenTelemetry Operator
3. Run script `./kube-deploy.sh up` to build and deploy services and sidecars
   - UserGroups API (`/usergroups`)
     Build Container from root
     ```shell
     docker build --tag ed-demo/dapr-usergroups-api:latest --file Services/UserGroups.Api/Dockerfile .
     ```


## Further Reading
[OpenTelemetry Collector - Getting Started](https://opentelemetry.io/docs/collector/getting-started/)





