﻿using System;
using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http上下文
    /// </summary>
    public class HttpContext : Disposable
    {
        /// <summary>
        /// 获取关联的HttpClient实例
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// 获取Api配置选项
        /// </summary>
        public HttpApiOptions Options { get; }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpApiRequestMessage RequestMessage { get; }

        /// <summary>
        /// 获取关联的的HttpResponseMessage
        /// </summary>
        public HttpResponseMessage? ResponseMessage { get; internal set; }

        /// <summary>
        /// http上下文
        /// </summary>
        /// <param name="client">httpClient</param>
        /// <param name="services">服务提供者</param>
        /// <param name="options">接口选项</param>
        public HttpContext(HttpClient client, IServiceProvider services, HttpApiOptions options)
        {
            this.Client = client;
            this.Services = services;
            this.Options = options;
            this.RequestMessage = new HttpApiRequestMessage(options.HttpHost ?? client.BaseAddress);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.RequestMessage?.Dispose();
        }
    }
}
