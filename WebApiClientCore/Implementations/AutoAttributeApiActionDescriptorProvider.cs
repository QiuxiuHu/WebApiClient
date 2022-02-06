﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 当参数空缺特性时
    /// 根据请求方式自动填充合理的特性
    /// GET或HEAD请求应用PathQueryAttribute
    /// 其它请求时简单类型应用PathQueryAttribute复杂类型使用JsonContentAttribute
    /// </summary>
    public class AutoAttributeApiActionDescriptorProvider : IApiActionDescriptorProvider
    {
        /// <summary>
        /// 创建Action描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="interfaceType">接口类型</param> 
        public ApiActionDescriptor CreateApiActionDescriptor(MethodInfo method, Type interfaceType)
        {
            return new AutoAttributeApiActionDescriptor(method, interfaceType);
        }

        /// <summary>
        /// 根据请求方式自动填充合理的特性的ApiActionDescriptor
        /// </summary>
        private class AutoAttributeApiActionDescriptor : DefaultApiActionDescriptor
        {
            private static readonly IApiParameterAttribute pathQueryAttribute = new PathQueryAttribute();
            private static readonly IApiParameterAttribute jsonContentAttribute = new JsonContentAttribute();

            /// <summary>
            /// GET或HEAD请求应用PathQueryAttribute
            /// 其它请求时简单类型应用PathQueryAttribute复杂类型使用JsonContentAttribute
            /// </summary>
            /// <param name="method"></param>
            /// <param name="interfaceType"></param>
            public AutoAttributeApiActionDescriptor(MethodInfo method, Type interfaceType)
                : base(method, interfaceType)
            {
                var descriptors = new List<DefaultApiParameterDescriptor>();
                var isGetHeadMethod = this.Attributes.Any(a => IsGetHeadAttribute(a));

                foreach (var parameter in this.Parameters)
                {
                    var defaultAttribute = this.GetDefaultAttribute(isGetHeadMethod, parameter.ParameterType);
                    descriptors.Add(new DefaultApiParameterDescriptor(parameter.Member, defaultAttribute));
                }
                this.Parameters = descriptors.ToReadOnlyList();
            }

            /// <summary>
            /// 是否为Get或Head特性
            /// </summary>
            /// <param name="apiActionAttribute">方法特性</param>
            /// <returns></returns>
            private static bool IsGetHeadAttribute(IApiActionAttribute apiActionAttribute)
            {
                if (apiActionAttribute is HttpMethodAttribute methodAttribute)
                {
                    var httpMethod = methodAttribute.Method;
                    return httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Head;
                }
                return false;
            }

            /// <summary>
            /// 获取参数缺省特性时的默认特性
            /// </summary>
            /// <param name="parameterType">参数类型</param>
            /// <param name="isGetHeadMethod">是否为Get或Head请求方式</param>
            /// <returns></returns>
            private IApiParameterAttribute GetDefaultAttribute(bool isGetHeadMethod, Type parameterType)
            {
                if (isGetHeadMethod == true)
                {
                    return pathQueryAttribute;
                }

                var realType = Nullable.GetUnderlyingType(parameterType) ?? parameterType;
                return IsSimpleType(realType) ? pathQueryAttribute : jsonContentAttribute;
            }

            /// <summary>
            /// 是否为简单类型
            /// </summary>
            /// <param name="realType">真实类型</param>
            /// <returns></returns>
            private static bool IsSimpleType(Type realType)
            {
                return realType.IsPrimitive
                    || realType == typeof(string)
                    || realType == typeof(decimal)
                    || realType == typeof(DateTime)
                    || realType == typeof(DateTimeOffset)
                    || realType == typeof(Guid)
                    || realType == typeof(Uri)
                    || realType == typeof(Version);
            }
        }
    }
}
