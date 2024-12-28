# Kubernetes Deployment for the Microservice-based Grade Management System

This directory contains the Kubernetes manifest files needed to deploy the Grade Management System. Each service is deployed in a dedicated namespace and includes configurations for Deployments, Services, Horizontal Pod Autoscalers (HPA), and an API Gateway.

## 1. Cluster Setup and Namespace

All resources are deployed in the `mynetwork` namespace to keep them organized and isolated from other resources.

### Creating the Namespace

If the namespace does not already exist, it can be created as follows:

```bash
kubectl apply -f namespace.yaml
```

The namespace `mynetwork` is already specified in the manifest files, so all resources will be created within it.

## 2. Deployment and Service Details

Here are the details for each service’s Deployment, Service, and Horizontal Pod Autoscaler (HPA):

### 2.1 Authentication Service

- **Deployment Name**: `auth-service`
- **Image**: [`auth-service:latest`](https://hub.docker.com/repository/docker/kurizakii/auth-service/general)
- **Replicas**: 2
- **Service Type**: ClusterIP (internal)
- **Port**: 8080
- **HPA**: Configured with 2-5 replicas, scaling target: 50% CPU utilization

### 2.2 Calculation Service

- **Deployment Name**: `calc-service`
- **Image**: [`calc-service:latest`](https://hub.docker.com/repository/docker/kurizakii/calc-service/general)
- **Replicas**: 2
- **Service Type**: ClusterIP (internal)
- **Port**: 8080

- **Deployment Name**: `calculation-service`
- **Image**: [`calc-service:latest`](https://hub.docker.com/repository/docker/kurizakii/calc-service/general)
- **Replicas**: 2
- **Service Type**: ClusterIP (internal)
- **HPA**: Configured with 2-5 replicas, scaling target: 50% CPU utilization

### 2.3 Grade Service

- **Deployment Name**: `grade-service`
- **Image**: [`grade-service:latest`](https://hub.docker.com/repository/docker/kurizakii/grade-service/general)
- **Replicas**: 2
- **Service Type**: ClusterIP (internal)
- **Port**: 8080
- **HPA**: Configured with 2-5 replicas, scaling target: 50% CPU utilization

### 2.4 API Gateway

- **Deployment Name**: `api-gateway`
- **Image**: [`api-gateway:latest`](https://hub.docker.com/repository/docker/kurizakii/api-gateway/general)
- **Replicas**: 2
- **Service Type**: LoadBalancer
- **Port**: 8080 (external) -> 80 (internal)
- **HPA**: Configured with 2-5 replicas, scaling target: 50% CPU utilization

The API Gateway routes incoming requests to the internal services. The NGINX configuration for this gateway is stored in the `ApiGateway/conf` directory and is mounted into the container using a `hostPath` volume.

## 3. API Gateway Configuration

The API Gateway uses NGINX as a reverse proxy to route requests to internal services. The NGINX configuration file `apigateway.conf` is mounted into the NGINX container at `/etc/nginx/conf.d/`.

- **Static Files**: Frontend files (HTML, CSS, JavaScript) are stored in the `frontend` folder and mounted to `/usr/share/nginx/html` in the API Gateway container.
- **Access**: The `api-gateway` service is configured as a LoadBalancer, making it accessible externally on port 8080.

## 4. Horizontal Pod Autoscaler (HPA)

Each service has an HPA configured to automatically scale the number of pods based on CPU utilization:

- **API Gateway HPA**: 2-5 replicas, target 50% CPU utilization
- **Authentication Service HPA**: 2-5 replicas, target 50% CPU utilization
- **Calculation Service HPA**: 2-5 replicas, target 50% CPU utilization
- **Grade Service HPA**: 2-5 replicas, target 50% CPU utilization

## 5. Applying the Configuration

To apply all resources in the `k8s` directory, run the following command:

```bash
kubectl apply -f k8s/
```

### Checking Status

To check the status of the deployments and services in the `mynetwork` namespace:

```bash
kubectl get deployments,services -n mynetwork
```

If any issues occur, you can view the logs of a specific pod:

```bash
kubectl logs <pod-name> -n mynetwork
```
