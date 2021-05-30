﻿using System;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将参数值作为请求uri的特性  
    /// 要求必须修饰于第一个参数
    /// 支持绝对或相对路径
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class UriAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        /// <returns></returns>
        public Task BeforeRequestAsync(ApiParameterContext context)
        {
            if (context.ParameterValue == null)
            {
                throw new ArgumentNullException(context.Parameter.Name);
            }

            if (context.Parameter.Index > 0)
            {
                throw new HttpApiInvalidOperationException(Resx.invalid_UriAttribute);
            }

            var relative = new Uri(context.ParameterValue.ToString(), UriKind.RelativeOrAbsolute);
            if (relative.IsAbsoluteUri == true)
            {
                context.RequestMessage.RequestUri = relative;
            }
            else
            {
                var baseUri = context.RequestMessage.RequestUri;
                if (baseUri == null)
                {
                    throw new HttpApiInvalidOperationException(Resx.required_HttpHost);
                }
                context.RequestMessage.RequestUri = new Uri(baseUri, relative);
            }
            return Task.CompletedTask;
        }
    }
}
