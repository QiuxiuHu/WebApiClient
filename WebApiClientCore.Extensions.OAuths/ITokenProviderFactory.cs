﻿using System;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// Token提供者工厂接口
    /// </summary>
    public interface ITokenProviderFactory
    {
        /// <summary>
        /// 通过接口类型获取或创建其对应的token提供者
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <param name="typeMatchMode">类型匹配模式</param>     
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        ITokenProvider Create(Type httpApiType, TypeMatchMode typeMatchMode = TypeMatchMode.TypeOnly);

        /// <summary>
        /// 通过接口类型获取或创建其对应的token提供者
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <param name="typeMatchMode">类型匹配模式</param>
        /// <param name="name">TokenProvider的区分名称</param>     
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        ITokenProvider Create(Type httpApiType, TypeMatchMode typeMatchMode, string name);
    }
}
