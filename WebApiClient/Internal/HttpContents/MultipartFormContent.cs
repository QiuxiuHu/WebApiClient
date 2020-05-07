﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示form-data表单
    /// </summary>
    class MultipartFormContent : MultipartContent, IReadAsStringAsyncable
    {
        /// <summary>
        /// 分隔符
        /// </summary>
        private readonly string boundary;

        /// <summary>
        /// 省略文件内容的MultipartContent
        /// 此对象为MultipartFormContent子项的包装
        /// 需要缓存起来避免请求中GC将其回收影响到子项
        /// </summary>
        private Lazy<MultipartContent> ellipsisFileMultipartContent;

        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "multipart/form-data";

        /// <summary>
        /// form-data表单
        /// </summary>
        public MultipartFormContent()
            : this(Guid16.NewGuid16().ToString())
        {
        }

        /// <summary>
        /// form-data表单
        /// </summary>
        /// <param name="boundary">分隔符</param>
        public MultipartFormContent(string boundary)
            : base("form-data", boundary)
        {
            this.boundary = boundary;
            this.ellipsisFileMultipartContent = new Lazy<MultipartContent>(this.CreateEllipsisFileMultipartContent, true);

            var parameter = new NameValueHeaderValue("boundary", boundary);
            this.Headers.ContentType.Parameters.Clear();
            this.Headers.ContentType.Parameters.Add(parameter);
        }

        /// <summary>
        /// 读取为文本信息
        /// </summary>
        /// <returns></returns>
        Task<string> IReadAsStringAsyncable.ReadAsStringAsync()
        {
            return this.ellipsisFileMultipartContent.Value.ReadAsStringAsync();
        }

        /// <summary>
        /// 创建省略文件内容的MultipartContent
        /// </summary>
        /// <returns></returns>
        private MultipartContent CreateEllipsisFileMultipartContent()
        {
            var multipartContent = new MultipartFormContent(this.boundary);
            foreach (var httpContent in this)
            {
                if (httpContent is MulitpartFileContent fileContent)
                {
                    multipartContent.Add(fileContent.ToEllipsisFileContent());
                }
                else
                {
                    multipartContent.Add(httpContent);
                }
            }
            return multipartContent;
        }

    }
}
