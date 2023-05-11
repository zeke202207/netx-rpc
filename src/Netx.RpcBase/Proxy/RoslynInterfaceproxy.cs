using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Netx.RpcBase
{
    [Obsolete("放弃方案，暂时没有想到动态获取using的方法，无法动态添加using")]
    internal class RoslynInterfaceproxy
    {
        private static Lazy<RoslynInterfaceproxy> instance = new Lazy<RoslynInterfaceproxy>(() => new RoslynInterfaceproxy());

        private RoslynInterfaceproxy() { }

        /// <summary>
        /// 动态代理实例
        /// </summary>
        public static RoslynInterfaceproxy Instance
        {
            get
            {
                return instance.Value;
            }
        }

        /// <summary>
        /// 生成对待代理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public T Resolve<T>(InterceptorHandler handler)
            where T : class
        {
            Type type = BuildType<T>();
            return (T)Activator.CreateInstance(type, handler);
            //Type t = codeExtension.BuildType<T>();
            //var method = t.GetMethod("getName");
            //object obj = Activator.CreateInstance(t);
            //var result = method.Invoke(obj, new object[] { "张三" }).ToString();
            //Console.WriteLine(result);
            //return (T)obj;
        }

        /// <summary>
        /// 生成静态脚本
        /// </summary>
        /// <typeparam name="Tinteface"></typeparam>
        /// <returns></returns>
        private string GeneratorTypeCode<Tinteface>(string typeName)
        {
            var t = typeof(Tinteface);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("using System;");
            stringBuilder.Append($"using {t.Namespace};");
            stringBuilder.Append($"namespace {typeName}_namespace");
            stringBuilder.Append("{");
            stringBuilder.Append($"public class {typeName}:{t.Name}");
            stringBuilder.Append(" {");
            stringBuilder.Append($" private readonly NetxRpcBase.InterceptorHandler _handler; ");
            stringBuilder.Append($"  public {typeName}(NetxRpcBase.InterceptorHandler handler)");
            stringBuilder.Append("{");
            stringBuilder.Append($"_handler = handler;");
            stringBuilder.Append("}");
            MethodInfo[] targetMethods = t.GetMethods();
            foreach (MethodInfo targetMethod in targetMethods)
            {
                if (targetMethod.IsPublic)
                {
                    var returnType = targetMethod.ReturnType;
                    var parameters = targetMethod.GetParameters();
                    string pStr = string.Empty;
                    List<string> parametersName = new List<string>();
                    foreach (ParameterInfo parameterInfo in parameters)
                    {
                        var pType = parameterInfo.ParameterType;
                        pStr += $"{pType.Name} _{pType.Name},";
                        parametersName.Add($"_{pType.Name}");
                    }

                    stringBuilder.Append($"public {returnType.Name} {targetMethod.Name}({pStr.TrimEnd(',')})");
                    stringBuilder.Append(" {");
                    foreach (var pName in parametersName)
                    {
                        //stringBuilder.Append($"Intercept.Before({pName});");
                        stringBuilder.Append($" var result = _handler.InvokeMember(this, 0, nameof({targetMethod.Name}), new object[] {{ {pName} }});");
                    }
                    stringBuilder.Append($"return ({returnType.Name})result;");
                    stringBuilder.Append(" }");
                }
            }
            stringBuilder.Append(" }");
            stringBuilder.Append(" }");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 构建类对象
        /// </summary>
        /// <typeparam name="Tinteface"></typeparam>
        /// <returns></returns>
        private Type BuildType<Tinteface>()
        {
            var typeName = "_" + typeof(Tinteface).Name;
            var text = GeneratorTypeCode<Tinteface>(typeName);

            // 将代码解析成语法树
            SyntaxTree tree = SyntaxFactory.ParseSyntaxTree(text);

            var objRefe = MetadataReference.CreateFromFile(typeof(Object).Assembly.Location);
            var consoleRefe = MetadataReference.CreateFromFile(typeof(Tinteface).Assembly.Location);

            var compilation = CSharpCompilation.Create(
                syntaxTrees: new[] { tree },
                assemblyName: $"assembly{typeName}.dll",
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: AppDomain.CurrentDomain.GetAssemblies().Select(x => MetadataReference.CreateFromFile(x.Location)));

            Assembly compiledAssembly;
            using (var stream = new MemoryStream())
            {
                // 检测脚本代码是否有误
                var compileResult = compilation.Emit(stream);
                compiledAssembly = Assembly.Load(stream.GetBuffer());
            }
            return compiledAssembly.GetTypes().FirstOrDefault(c => c.Name == typeName);
        }
    }
}
