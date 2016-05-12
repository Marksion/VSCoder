Docker Compose 编排
==================

Compose是什么？
-------------
Compose是Docker公司提供的开源的编排部署工具。Compose的前身是Fig，使用Python代码编写。 License为Apache2.0，对商业友好。


为什么需要Compose？
-----------------
因为用户的实际应用是复杂的，可能由多个容器组成，容器间存在关系，使用上可能需要重复多次部署。
因此，需要一定的编排部署能力来简化这个操作。


Compose的架构
------------
Compose目前只提供命令行工具，没有Daemon存在。Compose与Docker紧密结合，目前只支持Docker。 当然Compose也可以支持Swarm。
Compose运行需要有对应的yml文件以及Dockerfile。
通过调用Docker/Swarm的API进行容器应用的编排。


Compose的现状
------------
官方推荐在开发、测试、持续集成等非生成环境使用。并不适合生产环境使用。


模型
----
service：实际上是一个包含某种功能的容器。 Compose的所有操作对象均为service。


功能
----
1.  build(构建yml中某个服务的镜像)
如上，web这个服务是依赖于镜像build的。在本地 也存在Dockerfile文件。
可以使用docker-compose build来构建服务的镜像。  
-  ps(查看已经启动的服务状态）
- kill(停止某个服务）
- logs(可以查看某个服务的log）
- port(打印绑定的public port）  
- pull(pull服务镜像)  
- up(启动yml定义的所有服务）
- stop(停止yml中定义的所有服务）
- start(启动被停止的yml中的所有服务）  
- kill(强行停止yml中定义的所有服务）
- rm（删除yml中定义的所有服务）
- restart(重启yml中定义的所有服务）  
- scale(扩展某个服务的个数，可以向上或向下）
- migrate-to-labels(重启容器并给容器加上 lable, 1.3之后 compose 使用 docker lable 来管理容器编排）
- version（查看compose的版本）


###例子
使用 Docker Compose 构建 Golang 开发环境

simple-golang-app/main.go
```
package main

import "fmt"

func main() {
  fmt.Println("Hello, World")
}
```

docker-compose.yml
```
app:
  image: golang:1.4
  working_dir: /go/src/simple-golang-app
  command: go run main.go
  volumes:
    - ./simple-golang-app:/go/src/simple-golang-app
```

```
$ docker-compose up
Creating dockercomposeexample_app_1...
Attaching to dockercomposeexample_app_1
app_1 | Hello, World
dockercomposeexample_app_1 exited with code 0
Gracefully stopping... (press Ctrl+C again to force)
```


database-golang-app/main.go
```
package main

import (
  "fmt"
  "gopkg.in/mgo.v2"
  "gopkg.in/mgo.v2/bson"
  "os"
  "time"
)

type Ping struct {
  Id   bson.ObjectId `bson:"_id"`
  Time time.Time     `bson:"time"`
}

func main() {
  // get the session using information from environment, ignore errors
  session, _ := mgo.Dial(os.Getenv("DATABASE_PORT_27017_TCP_ADDR"))
  db := session.DB(os.Getenv("DB_NAME"))
  defer session.Close()

  // insert new record
  ping := Ping{
    Id:   bson.NewObjectId(),
    Time: time.Now(),
  }
  db.C("pings").Insert(ping)

  // get all records
  pings := []Ping{}
  db.C("pings").Find(nil).All(&pings)

  fmt.Println(pings)
}
```

database-golang-app/Dockerfile
```
ROM golang:1.5

RUN go get gopkg.in/mgo.v2
```

docker-compose.yaml
```
advanced:
  build: ./database-golang-app
  working_dir: /go/src/database-golang-app
  command: go run main.go
  volumes:
    - ./database-golang-app:/go/src/database-golang-app
  links:
    - database
  environment:
    - DB_NAME=advanced-golang-db
database:
  image: mongo:3.0
  command: mongod --smallfiles --quiet --logpath=/dev/null
```

结果：
```
$ docker-compose up
Creating dockercomposeexample_database_1...
Creating dockercomposeexample_advanced_1...
Attaching to dockercomposeexample_database_1, dockercomposeexample_advanced_1
advanced_1 | [{ObjectIdHex("54efd8f889120d000d000001") 2015-02-27 02:39:52.53 +0000 UTC}]
dockercomposeexample_advanced_1 exited with code 0
Gracefully stopping... (press Ctrl+C again to force)
Stopping dockercomposeexample_database_1...
```

换成 Docker 命令行执行步骤如下：
```
$ docker run -d --name database mongo:3.0
$ docker build -t database-golang-app ./database-golang-app
$ docker run --link database:database -e DB_NAME=advanced-golang-db \
    -v /home/lrp/Projects/2015/docker-compose-example/database-golang-app:/go/src/database-golang-app \
     database-golang-app go run /go/src/database-golang-app/main.go
```


### 参考资料
1. Compose文档  
http://docs.docker.com/compose/
2. Github地址   
https://github.com/docker/compose







