# netx-rpc
客户端Rpc框架，仅包装了客户端rpc调用，是客户端使用者无感调用远程方法

## 版本说明

- 类库采用.NET Standard2.0版本
	- 支持列表详情请参考：https://dotnet.microsoft.com/zh-cn/platform/dotnet-standard
	- 支持情况


| .NET 实现			| 版本支持 |
|---				|---|
| .NET 和 .NET Core | 2.0 <font color=red>√</font>  2.1 <font color=red>√</font> 2.2 <font color=red>√</font> 3.0 <font color=red>√</font> 3.1 <font color=red>√</font> 5.0 <font color=red>√</font>6.0 <font color=red>√</font> 7.0 <font color=red>√</font>|
| .NET Framework	| 4.6.1<font color=red>√</font> 4.6.2<font color=red>√</font> 4.7<font color=red>√</font> 4.7.1<font color=red>√</font> 4.8<font color=red>√</font> 4.8.1<font color=red>√</font>|

## 使用方法

```
            using (var channle = RpcFactory.CreateRpcFactory<IFoo, RestfulConfigModel>(typeof(RestfulChannel), new RestfulRpcModel()
            {
                Config = new ConfigHandler()
            }))
            {
                var ifoo = channle.CreateRpcChannel<IFoo>(new RestfulInterceptorHandler());
                var result0 = ifoo.Foo1("hello zeke!", "hello zeke1!");
                var result = ifoo.Foo("hello zeke!");
                var result1 = ifoo.Bar(new Persion() { Id = "1", Name = "zeke" });
            }
```

## 说明

- 提供了Rpc基类封装
- 提供了Restful方式的http简单封装（可在此基础上进行扩展）
- 如果需要其他协议封装，可参考Restful封装方式进行处理

Let us enjoy it!!!
