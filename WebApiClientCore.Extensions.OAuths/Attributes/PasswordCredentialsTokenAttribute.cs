﻿using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Extensions.OAuths;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示由password模式token提供者提供的token应用特性
    /// 需要注册services.AddPasswordCredentialsTokenProvider
    /// </summary> 
    public class PasswordCredentialsTokenAttribute : OAuthTokenAttribute
    {
        /// <summary>
        /// 获取token提供者
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ITokenProvider GetTokenProvider(ApiRequestContext context)
        {
            var provider = base.GetTokenProvider(context);
            if (provider.ProviderType != ProviderType.PasswordClientCredentials)
            {
                throw new ApiInvalidConfigException($"未注册{nameof(TokenProviderExtensions.AddPasswordCredentialsTokenProvider)}");
            }
            return provider;
        }
    }
}
