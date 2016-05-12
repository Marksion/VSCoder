持续集成过程中的Docker使用介绍
==========================

##什么是持续交付?
持续交付是当前一个挺火的概念，它所描述的软件开发，是在从原始需求识别到最终产品部署到生产环境这个过程中，需求以小批量形式在团队的各个角色间顺畅流动，能够以较短地周期完成需求的小粒度频繁交付。频繁的交付周期带来了更迅速的对软件的反馈，并且在这个过程中，需求分析、产品的用户体验和交互设计、开发、测试、运维等角色密切协作，相比于传统的瀑布式软件团队，更少浪费。

简单来说，**持续交付是一种让产品代码快速更新迭代的方法**。


##容器时代的持续交付
当软件开发来到容器时代时，持续交付变得更加有其用武之地。Docker 的发明使得软件的构建、发布、部署和迁移变得尤为简单。Docker 通过容器技术，将所有的运行环境、组件依赖、运行管理做到了粒度小、可维护性高、迁移方便。

通过 Docker 的对应用代码的构建成为 Docker 镜像，Docker 镜像便是容器时代标准的交付件，只需将镜像推送到相应的位置，便可以实现测试、发布和部署等生产流程。

一份 Dockerfile 将镜像的构建内容安排妥当，包括运行的环境、依赖库的安装、代码的处理等等；利用 Jenkins CI script 安排镜像的自动拉取和测试任务。


## 集成工具 Jenkins vs Drone
```
/etc/drone/dronerc
REMOTE_DRIVER=github
REMOTE_CONFIG=https://github.com?client_id=${client_id}&client_secret=${client_secret}
DATABASE_DRIVER=sqlite3
DATABASE_CONFIG=/var/lib/drone/drone.sqlite
```

```
sudo docker run \
  --volume /var/lib/drone:/var/lib/drone \
  --volume /var/run/docker.sock:/var/run/docker.sock \
  --env-file /etc/drone/dronerc \
  --restart=always \
  --publish=80:8000 \
  --detach=true \
  --name=drone \
  drone/drone:0.4
```


### 参考资料：

http://tleyden.github.io/blog/2016/02/15/setting-up-a-self-hosted-drone-dot-io-ci-server/
