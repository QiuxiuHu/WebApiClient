﻿#if AOT
using System;
using System.Linq;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi代理描述
    /// 提供HttpApi代理类的实例化
    /// </summary>
    partial class HttpApiProxy
    {
        /// <summary>
        /// 接口类型与代理描述缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, HttpApiProxy> interfaceProxyCache = new ConcurrentCache<Type, HttpApiProxy>();

        /// <summary>
        /// 搜索接口的代理类型并实例化
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="interceptor">拦截器</param>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        public static HttpApi CreateInstance(Type interfaceType, IApiInterceptor interceptor)
        {
            var httpApiProxy = interfaceProxyCache.GetOrAdd(interfaceType, @interface =>
            {
                var proxyType = FindProxyType(@interface);
                if (proxyType == null)
                {
                    var assemblyName = typeof(HttpApiProxy).GetTypeInfo().Assembly.GetName();
                    throw new TypeLoadException($"找不到接口{interfaceType}的代理类，请为接口所在项目重新使用Nuget安装{assemblyName.Name} {assemblyName.Version}");
                }

                var apiMethods = @interface.GetAllApiMethods();
                return new HttpApiProxy(proxyType, apiMethods);
            });

            return httpApiProxy.CreateInstance(interceptor);
        }


        /// <summary>
        /// 查找代理类型
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        private static Type FindProxyType(Type interfaceType)
        {
            var allTypes = interfaceType.GetTypeInfo().Assembly.GetTypes();
            if (interfaceType.GetTypeInfo().IsGenericType == false)
            {
                return allTypes.FirstOrDefault(item => IsProxyType(interfaceType, item));
            }
            else
            {
                var definition = interfaceType.GetGenericTypeDefinition();
                var targetType = allTypes.FirstOrDefault(item => IsProxyType(definition, item));
                return targetType?.MakeGenericType(interfaceType.GenericTypeArguments);
            }
        }

        /// <summary>
        /// 返回target类型是否为代理类型
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="targetType">目标类型 </param>
        /// <returns></returns>
        private static bool IsProxyType(Type interfaceType, Type targetType)
        {
            if (targetType.Namespace != interfaceType.Namespace)
            {
                return false;
            }

            if (targetType.DeclaringType != interfaceType.DeclaringType)
            {
                return false;
            }

            const string prefix = "$";
            var proxyTypeName = $"{prefix}{interfaceType.Name}";
            return targetType.Name == proxyTypeName;
        }
    }
}
#endif