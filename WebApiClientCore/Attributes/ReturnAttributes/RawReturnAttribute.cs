﻿using System.Net;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示原始类型的结果特性
    /// 支持结果类型为string、byte[]、Stream和HttpResponseMessage
    /// </summary> 
    public sealed class RawReturnAttribute : SpecialReturnAttribute
    {
        /// <summary>
        /// 获取或设置是否确保响应的http状态码通过IsSuccessStatusCode验证
        /// 当值为true时，请求可能会引发HttpStatusFailureException
        /// 默认为true
        /// </summary>
        public bool EnsureSuccessStatusCode { get; set; } = true;

        /// <summary>
        /// 原始类型的结果特性
        /// </summary>
        public RawReturnAttribute()
            : base()
        {
        }

        /// <summary>
        /// 原始类型的结果特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public RawReturnAttribute(double acceptQuality)
            : base(acceptQuality)
        {
        }

        /// <summary>
        /// 响应时
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnResponseAsync(ApiResponseContext context)
        {
            var dataType = context.ApiAction.Return.DataType;
            var response = context.HttpContext.ResponseMessage;

            if (dataType.IsRawType == false || response == null)
            {
                return;
            }

            if (this.EnsureSuccessStatusCode == true)
            {
                if (this.IsSuccessStatusCode(response.StatusCode) == false)
                {
                    throw new ApiResponseStatusException(response);
                }
            }

            if (dataType.IsRawHttpResponseMessage == true)
            {
                context.Result = response;
            }
            else if (dataType.IsRawString == true)
            {
                context.Result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else if (dataType.IsRawByteArray == true)
            {
                context.Result = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
            else if (dataType.IsRawStream == true)
            {
                context.Result = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 指示http状态码是否为成功的状态码
        /// </summary>
        /// <param name="statusCode">http状态码</param>
        /// <returns></returns>
        private bool IsSuccessStatusCode(HttpStatusCode statusCode)
        {
            var status = (int)statusCode;
            return status >= 200 && status <= 299;
        }
    }
}
