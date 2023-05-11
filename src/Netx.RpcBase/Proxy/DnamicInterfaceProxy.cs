using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Netx.RpcBase
{
    internal class DnamicInterfaceProxy
    {
        private static Lazy<DnamicInterfaceProxy> instance = new Lazy<DnamicInterfaceProxy>(() => new DnamicInterfaceProxy());
        private static readonly ConcurrentDictionary<Type, Type> Maps = new ConcurrentDictionary<Type, Type>();

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
        public T Resolve<T>(InterceptorHandler handler)
            where T : class
        {
            var interfaceType = typeof(T);
            if (interfaceType?.IsInterface != true)
                throw new ArgumentException($"{nameof(interfaceType)} is not a interface");
            Maps.TryGetValue(interfaceType, out var newType);
            if (null == newType)
            {
                newType = CreateType(interfaceType);
                Maps.TryAdd(interfaceType, newType);
            }
            return (T)Activator.CreateInstance(newType, handler);
        }

        #region 

        /// <summary>
        /// 创建代理对象
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        private Type CreateType(Type interfaceType)
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
            CreateConstructor(typeBuilder, fieldBuilder);
            CreateMethods(interfaceType, typeBuilder, fieldBuilder);
            CreateProperties(interfaceType, typeBuilder, fieldBuilder);
            return typeBuilder.CreateTypeInfo().AsType();
        }

        /// <summary>
        /// 创建构造函数
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="fieldBuilder"></param>
        private void CreateConstructor(TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(InterceptorHandler) });
            var il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldBuilder);
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
