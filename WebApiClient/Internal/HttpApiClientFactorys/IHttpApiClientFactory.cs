﻿using System;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpApiClientFactory的接口
    /// </summary>
    interface IHttpApiClientFactory
    {
        /// <summary>
        /// 获取或设置生命周期
        /// </summary>
        TimeSpan Lifetime { get; set; }

        /// <summary>
        /// 当有记录失效时
        /// </summary>
        /// <param name="entry">激活状态的记录</param>
        void OnEntryDeactivate(ActiveHandlerEntry entry);
    }
}
