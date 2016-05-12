Docker API使用介绍
=================

sysV 和 Upstart 的系统

```
$ sudo echo "DOCKER_OPTS='$DOCKER_OPTS -H tcp://0.0.0.0:2375 -H unix:///var/run/docker.sock'\n" >> /etc/default/docker"
$ sudo service docker restart
```

支持 systemd 的 CentOS 配置如下：

touch /etc/systemd/system/docker-tcp.socket
```
[Unit]
Description=Docker Socket for the API

[Socket]
ListenStream=2375
BindIPv6Only=both
Service=docker.service

[Install]
WantedBy=sockets.target
```
激活socket
```
systemctl enable docker-tcp.socket
systemctl stop docker
systemctl start docker-tcp.socket
systemctl start docker
```

测试：
```
docker -H tcp://127.0.0.1:2375 ps


docker -H tcp://127.0.0.1:2375 version

curl -X GET http://127.0.0.1:2375/images/json|python -mjson.tool

```


###参考
https://docs.docker.com/engine/reference/api/docker_remote_api/



