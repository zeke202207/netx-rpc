# netx-rpc
�ͻ���Rpc��ܣ�����װ�˿ͻ���rpc���ã��ǿͻ���ʹ�����޸е���Զ�̷���

## �汾˵��

- ������.NET Standard2.0�汾
	- ֧���б�������ο���https://dotnet.microsoft.com/zh-cn/platform/dotnet-standard
	- ֧�����


| .NET ʵ��			| �汾֧�� |
|---				|---|
| .NET �� .NET Core | 2.0 <font color=red>��</font>  2.1 <font color=red>��</font> 2.2 <font color=red>��</font> 3.0 <font color=red>��</font> 3.1 <font color=red>��</font> 5.0 <font color=red>��</font>6.0 <font color=red>��</font> 7.0 <font color=red>��</font>|
| .NET Framework	| 4.6.1<font color=red>��</font> 4.6.2<font color=red>��</font> 4.7<font color=red>��</font> 4.7.1<font color=red>��</font> 4.8<font color=red>��</font> 4.8.1<font color=red>��</font>|

## ʹ�÷���

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

## ˵��

- �ṩ��Rpc�����װ
- �ṩ��Restful��ʽ��http�򵥷�װ�����ڴ˻����Ͻ�����չ��
- �����Ҫ����Э���װ���ɲο�Restful��װ��ʽ���д���

Let us enjoy it!!!
