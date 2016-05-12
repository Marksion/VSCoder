Docker 镜像仓库搭建
------------------
运行下面命令获取registry镜像
```
$ sudo docker pull registry:2.3.0
```
然后启动一个容器，
```
$ sudo docker run -d -v /opt/registry:/var/lib/registry -p 5000:5000 --restart=always --name registry registry:2.3.0
```
Registry服务默认会将上传的镜像保存在容器的/var/lib/registry，我们将主机的/opt/registry目录挂载到该目录，即可实现将镜像保存到主机的/opt/registry目录了。

运行docker ps看一下容器情况，
```
$ sudo docker ps 
CONTAINER ID        IMAGE               COMMAND                  CREATED             STATUS              PORTS                    NAMES
f3766397a458        registry:2.3.0      "/bin/registry /etc/d"   46 seconds ago      Up 45 seconds       0.0.0.0:5000->5000/tcp   registry
```


我们参考一个国内镜像市场的地址：

```
$ curl https://index.alauda.cn/v2 -k -IL
HTTP/1.1 301 Moved Permanently
Content-Type: text/plain; charset=utf-8
Date: Sun, 21 Feb 2016 07:51:43 GMT
Docker-Distribution-Api-Version: registry/2.0
Location: /v2/
Server: nginx/1.4.6 (Ubuntu)
Connection: keep-alive

HTTP/1.1 401 Unauthorized
Content-Length: 114
Content-Type: application/json; charset=utf-8
Date: Sun, 21 Feb 2016 07:51:43 GMT
Docker-Distribution-Api-Version: registry/2.0
Server: nginx/1.4.6 (Ubuntu)
Www-Authenticate: Bearer realm="https://api.alauda.cn/v1/docker_auth/token/",service="index.alauda.cn"
Connection: keep-alive
```


构建一个认证的镜像市场
MacOS:

```
$ docker-machine ssh default
$ sudo vi /var/lib/boot2docker/profile
--insecure-registry 192.168.99.100:5000

$ sudo /etc/init.d/docker restart


```




参考：
====
1. https://the.binbashtheory.com/creating-private-docker-registry-2-0-with-token-authentication-service/

2. https://docs.docker.com/engine/security/certificates/

3. https://docs.docker.com/registry/insecure/