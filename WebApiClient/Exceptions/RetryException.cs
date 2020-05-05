﻿using System;

namespace WebApiClient
{
    /// <summary>
    /// 表示Http请求重试异常
    /// </summary>
    public class RetryException : HttpApiException
    {
        /// <summary>
        /// 获取重试的最大次数
        /// </summary>
        public int MaxRetryCount { get; }

        /// <summary>
        /// 重试异常
        /// </summary>
        /// <param name="maxRetryCount">重试的最大次数</param>   
        /// <param name="inner">内部异常</param>
        public RetryException(int maxRetryCount, Exception inner)
            : this(maxRetryCount, inner, $"重试已经达到了最大次数限制：{maxRetryCount}")
        {
        }

        /// <summary>
        /// 重试异常
        /// </summary>
        /// <param name="maxRetryCount">已重试的次数</param>        
        /// <param name="inner">内部异常</param>
        /// <param name="message">提示</param>
        public RetryException(int maxRetryCount, Exception inner, string message)
            : base(message, inner)
        {
            this.MaxRetryCount = maxRetryCount;
        }
    }
}
