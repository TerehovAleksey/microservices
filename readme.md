# Docker

### собрать образ
```sh
docker build -t terehoff/platformservice .
```
### опубликовать
```sh
docker push terehoff/platformservice
```

```sh
docker run -p 8080:80 terehoff/commandservice
```

# Kubernetes

```sh
kubectl get deployments
kubectl get pods
kubectl get services

kubectl apply  -f platforms-depl.yaml
```
## MSSQL
```sh
kubectl get storageclass
kubectl get pvc
kubectl create secret generic mssql --from-literal=SA_PASSWORD="pa55w0rd!"
```

### перезагрузить образ
```sh
kubectl rollout restart deployment platforms-depl
```

### удалить
```sh
kubectl delete deployment platforms-depl
```

# Ingress Nginx
https://kubernetes.github.io/ingress-nginx/

```sh
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.2.0/deploy/static/provider/cloud/deploy.yaml
```

```sh
kubectl get services --namespace=ingress-nginx
```