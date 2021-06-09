﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示http请求方法描述特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [DebuggerDisplay("Method = {Method}")]
    public class HttpMethodAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 获取请求方法
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// 获取请求相对路径
        /// </summary>
        public string? Path { get; }

        /// <summary>
        /// http请求方法描述特性
        /// </summary>
        /// <param name="method">请求方法</param>
        public HttpMethodAttribute(string method)
            : this(new HttpMethod(method))
        {
        }

        /// <summary>
        /// http请求方法描述特性
        /// </summary>
        /// <param name="method">请求方法</param>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpMethodAttribute(string method, string? path)
            : this(new HttpMethod(method), path)
        {
        }

        /// <summary>
        /// http请求方法描述特性
        /// </summary>
        /// <param name="method">请求方法</param>
        protected HttpMethodAttribute(HttpMethod method)
            : this(method, null)
        {
        }

        /// <summary>
        /// http请求方法描述特性
        /// </summary>
        /// <param name="method">请求方法</param>
        /// <param name="path">请求绝对或相对路径</param>
        protected HttpMethodAttribute(HttpMethod method, string? path)
        {
            this.Method = method;
            this.Path = path;
            this.OrderIndex = int.MinValue + 1;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var baseUri = context.HttpContext.RequestMessage.RequestUri;
            var relative = string.IsNullOrEmpty(this.Path) ? null : new Uri(this.Path, UriKind.RelativeOrAbsolute);
            var requestUri = this.GetRequestUri(baseUri, relative);

            context.HttpContext.RequestMessage.Method = this.Method;
            context.HttpContext.RequestMessage.RequestUri = requestUri;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取请求URL
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="relative"></param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
        private Uri? GetRequestUri(Uri? baseUri, Uri? relative)
        {
            if (baseUri == null)
            {
                if (relative == null || relative.IsAbsoluteUri == true)
                {
                    return relative;
                }
                throw new ApiInvalidConfigException(Resx.required_HttpHost);
            }
            else
            {
                return relative == null ? baseUri : new Uri(baseUri, relative);
            }
        }
    }
}
