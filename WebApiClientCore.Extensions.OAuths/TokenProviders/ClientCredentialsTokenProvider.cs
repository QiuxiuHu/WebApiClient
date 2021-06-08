﻿using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.OAuths.Exceptions;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示Client身份信息token提供者
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class ClientCredentialsTokenProvider<THttpApi> : TokenProvider, IClientCredentialsTokenProvider<THttpApi>
    {
        /// <summary>
        /// 身份选项
        /// </summary>
        protected IOptions<ClientCredentialsOptions<THttpApi>> CredentialsOptions { get; }

        /// <summary>
        /// Client身份信息token提供者
        /// </summary>
        /// <param name="services"></param> 
        /// <param name="credentialsOptions"></param>
        public ClientCredentialsTokenProvider(IServiceProvider services, IOptions<ClientCredentialsOptions<THttpApi>> credentialsOptions)
            : base(services)
        {
            this.CredentialsOptions = credentialsOptions;
        }

        /// <summary>
        /// 请求获取token
        /// </summary> 
        /// <param name="oAuthClient">Token客户端</param>
        /// <returns></returns>
        protected override Task<TokenResult?> RequestTokenAsync(IOAuthClient oAuthClient)
        {
            var options = this.CredentialsOptions.Value;
            if (options.Endpoint == null)
            {
                throw new TokenEndPointNullException();
            }
            return oAuthClient.RequestTokenAsync(options.Endpoint, options.Credentials);
        }

        /// <summary>
        /// 刷新token
        /// </summary> 
        /// <param name="oAuthClient">Token客户端</param>
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected override Task<TokenResult?> RefreshTokenAsync(IOAuthClient oAuthClient, string? refresh_token)
        {
            var options = this.CredentialsOptions.Value;
            if (options.Endpoint == null)
            {
                throw new TokenEndPointNullException();
            }

            var credentials = new RefreshTokenCredentials
            {
                Client_id = options.Credentials.Client_id,
                Client_secret = options.Credentials.Client_secret,
                Extra = options.Credentials.Extra,
                Refresh_token = refresh_token
            };

            return oAuthClient.RefreshTokenAsync(options.Endpoint, credentials);
        }
    }
}
