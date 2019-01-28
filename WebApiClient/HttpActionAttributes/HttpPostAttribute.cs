﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示Post请求
    /// 不可继承
    /// </summary>
    public sealed class HttpPostAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Post请求
        /// </summary>
        public HttpPostAttribute()
            : base(HttpMethod.Post)
        {
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpPostAttribute(string path)
            : base(HttpMethod.Post, path)
        {
        }
    }
}
