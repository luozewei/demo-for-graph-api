# demo-for-graph-api
## Microsoft Graph 简单使用分享(代码是今天临时赶出来的，严谨以及命名请大家见谅 2017-11-03)
主要包含了集成登陆了Office365国际和国内的支持<br>
缓存刷新token<br>
通过Graph APi 获取Office365中的用户<br>
通过Graph APi 获取Meeting信息<br>
通过Graph APi 获取邮件列表<br>
通过Graph APi 发送带附件邮件<br>
通过Graph APi 列出OneDrive上的最近使用的文档<br>
如果有疑问或问题请联系我<br>


## Graph API使用证书凭据的无人值守服务和Skype API 简单使用分享(Demo严谨以及命名请大家见谅 2018-3-16)
### Skype
https://localhost:44342/SkypeDemo/Index  页面为 Skype API 里面包含  获取状态、联系人、查询、创建虚拟会议、虚拟会议怎么和Graph 中的邮件关联。
关于Skype 需要注意的是 需要开启 app 的开启隐式授权，[参考(https://docs.azure.cn/zh-cn/active-directory/develop/active-directory-dev-understanding-oauth2-implicit-grant)]

### 证书凭据的无人值守
这里的Demo不会直接是一个无人值守，只i会告诉你怎么去获取无人值守的Token。以及Token的使用
[证书参考(https://docs.microsoft.com/zh-cn/azure/active-directory/develop/active-directory-certificate-credentials)]
第一 向管理员申请权限
第二 获取Token
后面大家脑补，有Token 还不会掉API。
