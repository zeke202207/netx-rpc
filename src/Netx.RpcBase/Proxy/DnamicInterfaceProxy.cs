using Netx.RpcBase.Models;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Netx.RpcBase
{
    public sealed class DnamicInterfaceProxy
    {
        private static Lazy<DnamicInterfaceProxy> instance = new Lazy<DnamicInterfaceProxy>(() => new DnamicInterfaceProxy());
        private readonly ConcurrentDictionary<Type, Type> Maps = new ConcurrentDictionary<Type, Type>();
        private static object objLock = new object();

        private DnamicInterfaceProxy() { }

        /// <summary>
        /// 动态代理实例
        /// </summary>
        public static DnamicInterfaceProxy Instance
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
        internal T Resolve<T,U>(InterceptorHandler handler , RpcModel<U> model)
            where T : class
            where U : ConfigModel
        {
            var interfaceType = typeof(T);
            if (interfaceType?.IsInterface != true)
                throw new ArgumentException($"{nameof(interfaceType)} is not a interface");
            Maps.TryGetValue(interfaceType, out var newType);
            if (null == newType)
            {
                lock (objLock)
                {
                    Maps.TryGetValue(interfaceType, out newType);
                    if (null == newType)
                    {
                        newType = CreateType(interfaceType, model);
                        Maps.TryAdd(interfaceType, newType);
                    }
                }
            }
            return (T)Activator.CreateInstance(newType, handler, model);
        }

        /// <summary>
        /// 获取代理Type
        /// </summary>
        /// <param name="key">字典key</param>
        /// <returns></returns>
        public Type GetCacheType(string key)
        {
            return Maps.FirstOrDefault(p=>p.Key.FullName == key).Value;
        }

        #region 

        /// <summary>
        /// 创建代理对象
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        private Type CreateType<T>(Type interfaceType, RpcModel<T> model)
            where T : ConfigModel
        {
            var assemblyName = new AssemblyName(nameof(DnamicInterfaceProxy));
            assemblyName.Version = new Version("1.0.0");
            assemblyName.CultureName = CultureInfo.CurrentCulture.Name;
            assemblyName.SetPublicKeyToken(new Guid().ToByteArray());
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(nameof(DnamicInterfaceProxy));
            var typeBuilder = moduleBuilder.DefineType($"{typeof(DnamicInterfaceProxy).FullName}.{interfaceType.Name}", TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(interfaceType);
            //create field
            var fieldBuilder = typeBuilder.DefineField("_handler", typeof(InterceptorHandler), FieldAttributes.Private);
            var modelFieldBuilder = typeBuilder.DefineField("_model", typeof(RpcModel<T>), FieldAttributes.Private);
            CreateConstructor(typeBuilder, fieldBuilder , modelFieldBuilder);
            CreateMethods(interfaceType, typeBuilder, fieldBuilder);
            CreateProperties(interfaceType, typeBuilder, fieldBuilder);
            return typeBuilder.CreateTypeInfo().AsType();
        }

        /// <summary>
        /// 创建构造函数
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="fieldBuilder"></param>
        private void CreateConstructor(TypeBuilder typeBuilder, FieldBuilder fieldBuilder, FieldBuilder modelFieldBuilder)
        {
            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(InterceptorHandler), typeof(object) });
            var il = ctor.GetILGenerator();
            //拦截方法委托字段
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldBuilder);
            //定义model字段
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, modelFieldBuilder);
            //生成model属性


            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// 创建代理方法
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="typeBuilder"></param>
        /// <param name="fieldBuilder"></param>
        private void CreateMethods(Type interfaceType, TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            foreach (MethodInfo method in interfaceType.GetMethods())
            {
                CreateMethod(method, typeBuilder, fieldBuilder);
            }
        }

        /// <summary>
        /// 创建代理方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="typeBuilder"></param>
        /// <param name="fieldBuilder"></param>
        private MethodBuilder CreateMethod(MethodInfo method, TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var args = method.GetParameters();
            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig,
                method.CallingConvention,
                method.ReturnType,
                args.Select(p => p.ParameterType).ToArray());
            var il = methodBuilder.GetILGenerator();
            il.DeclareLocal(typeof(object[]));
            if (method.ReturnType != typeof(void))
                il.DeclareLocal(method.ReturnType);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldc_I4, args.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc_0);
            for (int i = 0; i < args.Length; i++)
            {
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, 1 + i);
                var type = args[i].ParameterType;
                if (type.IsValueType)
                    il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldBuilder);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4, method.MetadataToken);
            il.Emit(OpCodes.Ldstr, method.DeclaringType?.FullName + "+" + method.Name);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Callvirt, typeof(InterceptorHandler).GetMethod(nameof(InterceptorHandler.InvokeMember), BindingFlags.Instance | BindingFlags.Public));

            if (method.ReturnType == typeof(void))
                il.Emit(OpCodes.Pop);
            else
            {
                il.Emit(method.ReturnType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, method.ReturnType);
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldloc_1);
            }
            il.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        /// <summary>
        /// 创建代理属性
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="typeBuilder"></param>
        /// <param name="fieldBuilder"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CreateProperties(Type interfaceType, TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            foreach (var prop in interfaceType.GetProperties())
            {
                var propertyBuilder = typeBuilder.DefineProperty(prop.Name, PropertyAttributes.SpecialName, prop.PropertyType, Type.EmptyTypes);
                var method = prop.GetGetMethod();
                if (method != null)
                {
                    var methodBuilder = CreateMethod(method, typeBuilder, fieldBuilder);
                    propertyBuilder.SetGetMethod(methodBuilder);
                }
                method = prop.GetSetMethod();
                if (method != null)
                {
                    var methodBuilder = CreateMethod(method, typeBuilder, fieldBuilder);
                    propertyBuilder.SetSetMethod(methodBuilder);
                }
            }
        }

        #endregion
    }
}
