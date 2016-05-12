Docker安全部署的17条建议
=====================


##1. Docker镜像

Docker 1.3开始支持使用数字签名[3]来验证官方仓库镜像的来源和完整性。该功能仍在开发中，因此Docker只对（译者注：没有数字签名的）镜像发出警告信息但不会阻止其实际运行。此外，这点对非官方镜像不适用。

一般情况下，我们要确保只从受信任的库中获取镜像，并且不要使用--insecure-registry=[]参数。

## 2. 网络命名空间
默认情况下，Docker守护进程暴露出来用于控制容器的REST API只能在本地通过Unix Domain Socket进行访问。

在一个TCP端口上运行Docker（比如，启动Docker守护进程时使用-H选项强制绑定地址）将允许任何可以访问该端口的人获取容器的访问权限，甚至在本地用户属于Docker组的某些情况下有可能获得宿主的root权限。[5]

在允许通过TCP访问守护进程时，确保通讯使用SSL加密[6]和权限控制能有效地防止未授权用户与其进行交互。

可在Docker的标准网络桥接接口docker0上启用内核防火墙iptables规则，用于加强这些控制。

例如，可以使用以下iptables过滤器[7]限制Docker容器的源IP地址范围与外界通讯。iptables -t filter -A FORWARD -s <source_ip_range> -j REJECT --reject-with icmp-admin-prohibited


## 3. 日志和审核
收集并归档与Docker相关的安全日志来达到审核和监控的目的。可以在宿主[8]上使用以下命令在容器外部访问日志文件：
```
docker run -v /dev/log:/dev/log <container_name> /bin/sh
```
使用Docker内置命令：
```
docker logs ... (-f to follow log output)
```

日志文件也可以导出成一个压缩包实现持久存储：
```
docker export ...
```

## 4. SELinux 或 AppArmor

通过访问控制的安全策略，可以配置Linux内核安全模块，如安全增强型Linux（SELinux）和AppArmor，从而实现强制性的访问控制（MAC）用以将进程约束在一套有限的系统资源或权限中。

如果先前已经安装并配置过SELinux，那么可以在容器使用setenforce 1来启用它。Docker守护进程的SELinux功能默认是禁用的，需要使用--selinux-enabled来启用。

容器的标签限制可使用新增的—-security-opt加载SELinux或者AppArmor的策略进行配置，该功能在Docker版本1.3引入。

```
--security-opt="label:user:USER" : Set the label user for the container
--security-opt="label:role:ROLE" : Set the label role for the container
--security-opt="label:type:TYPE" : Set the label type for the container
--security-opt="label:level:LEVEL" : Set the label level for the container
--security-opt="apparmor:PROFILE" : Set the apparmor profile to be applied to the container 
```
例如：
```
docker run --security-opt=label:level:s0:c100,c200 -i -t centos bash
```

## 5. 守护进程特权

不要使用--privileged命令行选项。否则将允许容器访问宿主上的所有设备，另外，为容器提供特定的LSM（例如SELinux或AppArmor）配置，将给予它与运行在宿主上的进程同等访问权限。

避免使用--privileged有助于减少攻击面和可能的宿主威胁。但是，这并不意味着运行守护进程时不需要root权限，在最新版本中这仍然是必须的。

启动守护进程和容器的权限只能赋予受信任的用户。

可通过使用-u选项弱化容器内访问权限。例如：
```
docker run -u <username> -it <container_name> /bin/bash
```
Docker组的任何用户部分可能最终从容器中的主机上获得根源。

## 6. cgroups
为了防止通过耗尽系统资源引发拒绝服务（DoS）攻击，可使用特定的命令行参数被来启用一些资源限制。

CPU使用：
```
docker run -it --rm --cpuset=0,1 -c 2 ...
```

内存使用：
```
docker run -it --rm -m 128m ...
```

存储使用：
```
docker -d --storage-opt dm.basesize=5G 
```

磁盘I/O：
目前Docker不支持。通过systemd暴露的BlockIO*特性可以在支持的操作系统中用来控制磁盘使用配额。

**BlockIOWriteBandwidth** 和 **BlockIOReadBandwidth**

```
$ docker run -it --rm --name block-device-test fedora bash
bash-4.2# time $(dd if=/dev/zero of=testfile0 bs=1000 count=100000 && sync)
100000+0 records in
100000+0 records out
100000000 bytes (100 MB) copied, 0.202718 s, 493 MB/s

real  0m3.838s
user  0m0.018s
sys   0m0.213s
```
```
$ mount
/dev/mapper/docker-253:0-3408580-d2115072c442b0453b3df3b16e8366ac9fd3defd4cecd182317a6f195dab3b88 on / type ext4 (rw,relatime,context="system_u:object_r:svirt_sandbox_file_t:s0:c447,c990",discard,stripe=16,data=ordered)
proc on /proc type proc (rw,nosuid,nodev,noexec,relatime)
tmpfs on /dev type tmpfs (rw,nosuid,context="system_u:object_r:svirt_sandbox_file_t:s0:c447,c990",mode=755)

[SNIP]
```
通过BlockIOWriteBandwidth的配置，限制磁盘写速度到10M/s
```
$ sudo systemctl set-property --runtime docker-d2115072c442b0453b3df3b16e8366ac9fd3defd4cecd182317a6f195dab3b88.scope "BlockIOWriteBandwidth=/dev/mapper/docker-253:0-3408580-d2115072c442b0453b3df3b16e8366ac9fd3defd4cecd182317a6f195dab3b88 10M"
```

```
bash-4.2# time $(dd if=/dev/zero of=testfile0 bs=1000 count=100000 && sync)
100000+0 records in
100000+0 records out
100000000 bytes (100 MB) copied, 0.229776 s, 435 MB/s

real  0m10.428s
user  0m0.012s
sys   0m0.276s
```

##7. 二进制SUID/GUID
SUID和GUID程序在受攻击导致任意代码执行（如缓冲区溢出）时将非常危险，因为它们将运行在进程文件所有者或组的上下文中。

如果可能的话，使用特定的命令行参数减少赋予容器的能力，阻止SUID和SGID生效。
```
docker run -it --rm --cap-drop SETUID --cap-drop SETGID ...
```

还有种做法，可以考虑在挂载文件系统时使用nosuid属性来移除掉SUID能力。

最后一种做法是，删除系统中不需要的SUID和GUID程序。这类程序可在Linux系统中运行以下命令而找到：
```
find / -perm -4000 -exec ls -l {} \; 2>/dev/null
find / -perm -2000 -exec ls -l {} \; 2>/dev/null
```
然后，可以使用类似于下面的[11]命令将移除SUID和GUID文件权限：
```
$ sudo chmod u-s filename   
$ sudo chmod -R g-s directory
```
##8. 设备控制组(/dev/*)
如果需要，使用内置的--device选项（-v参数不要与--privileged一起使用）。此功能在1.2版本引入。

例如（使用声卡）：
```
docker run --device=/dev/snd:/dev/snd …
```

##9. 服务和应用
如果Docker容器有可能被入侵，为了减少横向运动的可能，应考虑隔离敏感服务（如在宿主或虚拟机上运行SSH服务）。

此外，不要在容器内使用root权限运行不受信任的应用。

##10. 挂载点
使用原生容器库（如libcontainer）时，Docker会自动处理这项。

但是，使用LXC容器库时，敏感的挂载点最好以只读权限手动挂载，包括：

> * /sys
> * /proc/sys
> * /proc/sysrq-trigger
> * /proc/irq
> * /proc/bus

挂载权限应在之后移除，以防止重新挂载。

##11. Linux内核
使用系统提供的更新工具（如apt-get、yum等）确保内核是最新的。过时的内核相比已公开的漏洞危险性更大。

使用GRSEC或PAX来强化内核，例如针对内存破坏漏洞提供更高的安全性。

CVE-2015-7547: glibc getaddrinfo stack-based buffer overflow


##12. 用户命名空间
Docker 1.10开始支持 user namespace。之前的版本不支持。

该功能允许Docker守护进程以非特权用户身份运行在宿主上，但在容器内看起来像是以root运行。
DEMO:
https://asciinema.org/a/5uyrknsjg7u2fad6ii0wgizd4?speed=2

##13. libseccomp（和seccomp-bpf 扩展）
libseccomp库允许基于白名单方法来限制Linux内核的系统调用程序的使用。最好禁用受攻击容器中对于系统操作不是很重要的系统调用程序，以防止其被滥用或误用。

Docker 1.10增强此功能，Demo:
https://asciinema.org/a/2rmol1dao8qf7tab5gcudq9ck

##14. 能力
尽可能降低Linux能力。Docker默认的能力包括：chown、dac_override、fowner、kill、setgid、setuid、setpcap、net_bind_service、net_raw、sys_chroot、mknod、setfcap、和audit_write`。

在命令行启动容器时，可以通过--cap-add=[]或--cap-drop=[]进行控制。

例如：
```
docker run --cap-drop setuid --cap-drop setgid -ti <container_name> /bin/sh
```

##15. 多租户环境

由于Docker容器内核的共享性质，无法在多租户环境中安全地实现责任分离。建议将容器运行在没有其它目的，且不用于敏感操作的宿主上。可以考虑将所有服务迁移到Docker控制的容器城。

可能的话，设置守护进程使用--icc=false，并根据需要在docker run时指定-link，或通过—-export=port暴露容器的一个端口，而不需要在宿主上发布。

将相互信任的容器的组映射到不同机器上

##16. 完全虚拟化
使用一个完全虚拟化解决方案来容纳Docker，如KVM。如果容器内的内核漏洞被发现，这将防止其从容器扩大到宿主上。

如同Docker-in-Docker工具所示，Docker镜像可以嵌套来提供该KVM虚拟层。

##17. 安全审核
定期对你的宿主系统和容器进行安全核查，以找出可能导致系统被入侵的错误配置或漏洞。




## 参考文献：

https://github.com/GDSSecurity/Docker-Secure-Deployment-Guidelines