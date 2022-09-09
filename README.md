# Dapr Demo

# Deploy OpenTelemetry Collector

$ kubectl apply -f https://raw.githubusercontent.com/open-telemetry/opentelemetry-collector/main/examples/k8s/otel-config.yaml

https://opentelemetry.io/docs/collector/getting-started/

# Fix Script Permissions
```shell
chmod +x kube-init.sh kube-init-down.sh kube-down.sh kube-up.sh
```

# Install cert-manager Command Line Tool
```shell
OS=$(go env GOOS); ARCH=$(go env GOARCH); curl -sSL -o cmctl.tar.gz https://github.com/cert-manager/cert-manager/releases/download/v1.7.2/cmctl-$OS-$ARCH.tar.gz
tar xzf cmctl.tar.gz
sudo mv cmctl /usr/local/bin
```

# Install helm


# Configure Open Telemetry Collector
1. Install Cert Manager
    ```shell
    kubectl apply -f https://github.com/cert-manager/cert-manager/releases/latest/download/cert-manager.yaml
    ```
2. Install Open Telemetry Operator
    ```shell
    kubectl apply -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml
    ``` 
3. Create Open Telemetry Collector
    ```shell
   
4. ```
4. 


# Tear Stack Down

kubectl delete \
   -f Infra/otel-collector.yaml \
   -f Services/UserGroups.Api/UserGroups.Api.yaml

Build Container from root
```shell
docker build --tag ed-demo/dapr-usergroups-api:latest --file Services/UserGroups.Api/Dockerfile .
```