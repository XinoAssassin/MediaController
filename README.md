这是一个Windows平台上简易的媒体遥控器

## 坑点

请自行添加 Windows 防火墙规则以及 netsh http urlacl 规则以正常使用。

## 用法

编译之后把 index.html 复制到与 MediaController.exe 相同的目录中，局域网内同网段任意设备访问 http://ip:8964 即可。

## 原理

调用 user32.dll 提供的 keybd_event，模拟进行键盘输入。