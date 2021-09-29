﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.Exceptions;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示Password模式的token提供者泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PasswordCredentialsTokenProvider<T> : TokenProvider
    {
        /// <summary>
        /// Password模式的token提供者
        /// </summary>
        /// <param name="services"></param>
        public PasswordCredentialsTokenProvider(IServiceProvider services)
            : base(services)
        {
        }

        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        protected override Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider)
        {
            var options = this.GetOptions<PasswordCredentialsOptions>();
            if (options.Endpoint == null)
            {
                throw new TokenEndPointNullException();
            }

            var clientApi = serviceProvider.GetRequiredService<IOAuthTokenClientApi>();
            return clientApi.RequestTokenAsync(options.Endpoint, options.Credentials);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="refresh_token"></param>
        /// <returns></returns>
        protected override Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
        {
            var options = this.GetOptions<PasswordCredentialsOptions>();
            if (options.Endpoint == null)
            {
                throw new TokenEndPointNullException();
            }

            if (options.UseRefreshToken == false)
            {
                return this.RequestTokenAsync(serviceProvider);
            }

            var refreshCredentials = new RefreshTokenCredentials
            {
                Client_id = options.Credentials.Client_id,
                Client_secret = options.Credentials.Client_secret,
                Extra = options.Credentials.Extra,
                Refresh_token = refresh_token
            };

            var clientApi = serviceProvider.GetRequiredService<IOAuthTokenClientApi>();
            return clientApi.RefreshTokenAsync(options.Endpoint, refreshCredentials);
        }
    }
}
