﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示multipart/form-data的一个文件项
    /// </summary>
    public class MulitpartFile : IHttpContentable
    {
        /// <summary>
        /// 流
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// 文件路径
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// multipart/form-data的一个文件项
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="fileName">文件友好名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MulitpartFile(Stream stream, string fileName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            this.stream = stream;
            this.FileName = fileName;
        }

        /// <summary>
        /// multipart/form-data的一个文件项
        /// </summary>
        /// <param name="localFilePath">本地文件路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public MulitpartFile(string localFilePath)
        {
            if (string.IsNullOrEmpty(localFilePath))
            {
                throw new ArgumentNullException();
            }

            if (File.Exists(localFilePath) == false)
            {
                throw new FileNotFoundException(localFilePath);
            }

            this.filePath = localFilePath;
            this.FileName = Path.GetFileName(localFilePath);
        }


        /// <summary>
        /// 设置http请求内容到上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的属性</param>
        void IHttpContentable.SetRquestHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var multipartForm = this.GetHttpContentFromContext(context);
            var content = this.ConvertToHttpContent();
            multipartForm.Add(content, parameter.Name, this.FileName);
            context.RequestMessage.Content = multipartForm;
        }

        /// <summary>
        /// 转换为HttpContent
        /// </summary>
        /// <returns></returns>
        private HttpContent ConvertToHttpContent()
        {
            if (this.stream != null)
            {
                return new StreamContent(this.stream);
            }
            else
            {
                var fileStream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read);
                return new StreamContent(fileStream);
            }
        }

        /// <summary>
        /// 从上下文获取已有MultipartFormDataContent
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private MultipartFormDataContent GetHttpContentFromContext(ApiActionContext context)
        {
            var httpContent = context.RequestMessage.Content as MultipartFormDataContent;
            if (httpContent == null)
            {
                httpContent = new MultipartFormDataContent();
            }
            return httpContent;
        }
    }
}
