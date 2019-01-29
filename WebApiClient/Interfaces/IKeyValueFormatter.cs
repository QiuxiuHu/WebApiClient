﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义参数转换为键值对的行为
    /// </summary>
    public interface IKeyValueFormatter
    {
        /// <summary>
        /// 序列化模型对象为键值对
        /// </summary>
        /// <param name="model">对象</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> Serialize(object model, FormatOptions options);

        /// <summary>
        /// 将参数值序列化为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> Serialize(ApiParameterDescriptor parameter, FormatOptions options);
    }
}
