﻿using System;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义ApiAction过滤器修饰特性的行为
    /// </summary>
    public interface IApiFilterAttribute : IAttributeMultiplable
    {
        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next">下一个执行委托</param>
        /// <returns></returns>
        Task OnRequestAsync(ApiRequestContext context, Func<Task> next);

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next">下一个执行委托</param>
        /// <returns></returns>
        Task OnResponseAsync(ApiResponseContext context, Func<Task> next);
    }
}
