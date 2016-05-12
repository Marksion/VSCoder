Docker push 流程
---------------

docker registry 在升级到 v2 后加入了很多安全相关检查，使得原来很多在 v1 already exist 的层依然要 push 到 registry,并且由于 v2 中的存储格式变成了 gzip，在镜像压缩过程中占用的时间很有可能比网络传输还要多。我们简单分解一下 docker push 的流程。

1. buffer to diske 将该层文件系统压缩成本地的一个临时文件
2. 上传文件至 registry
3. 本地计算压缩包 digest，删除临时文件，digest 传给 registry
4. registry 计算上传压缩包 digest 并进行校验
5. registry 将压缩包传输至后端存储文件系统
6. 重复 1-5 直至所有层传输完毕
7. 计算镜像的 manifest 并上传至 registry 重复 3-5

镜像推送的一般步骤：

```
$ sudo docker run -t -i ubuntu /bin/bash
root@9593c56f9e70:/# echo "TEST" >/mydockerimage
root@9593c56f9e70:/# exit
$ sudo docker commit $(sudo docker ps -lq) vinod-image
e17b685ee6987bb0cd01b89d9edf81a9fc0a7ad565a7e85650c41fc7e5c0cf9e
```

```
$ sudo docker login https://mydomain.com:8080
Username: vinod1
Password:
Email: vinod.puchi@gmail.com
Login Succeeded
```

```
$ sudo docker tag vinod-image mydomain.com:8080/vinod-image
$ sudo docker push mydomain.com:8080/vinod-image
```


