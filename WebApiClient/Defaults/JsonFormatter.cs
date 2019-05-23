﻿using Newtonsoft.Json;
using System;
using WebApiClient.DataAnnotations;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 默认的json解析工具
    /// </summary>
    public class JsonFormatter : IJsonFormatter
    {
        /// <summary>
        /// 使用CamelCase的json属性解析约定
        /// </summary>
        private readonly static PropertyContractResolver useCamelCaseResolver = new PropertyContractResolver(true, FormatScope.JsonFormat);

        /// <summary>
        /// 不使用CamelCase的json属性解析约定
        /// </summary>
        private readonly static PropertyContractResolver noCamelCaseResolver = new PropertyContractResolver(false, FormatScope.JsonFormat);

        /// <summary>
        /// 将对象列化为json文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public string Serialize(object obj, FormatOptions options)
        {
            if (obj == null)
            {
                return null;
            }

            if (options == null)
            {
                options = new FormatOptions();
            }

            var setting = this.CreateSerializerSettings(options);
            return JsonConvert.SerializeObject(obj, setting);
        }

        /// <summary>
        /// 反序列化json为对象
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        public object Deserialize(string json, Type objType)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var setting = this.CreateSerializerSettings(null);
            return JsonConvert.DeserializeObject(json, objType, setting);
        }

        /// <summary>
        /// 创建序列化或反序列化配置       
        /// </summary>
        /// <param name="options">格式化选项</param>
        /// <returns></returns>
        protected virtual JsonSerializerSettings CreateSerializerSettings(FormatOptions options)
        {
            var setting = new JsonSerializerSettings();
            if (options != null)
            {
                setting.DateFormatString = options.DateTimeFormat;
                setting.ContractResolver = options.UseCamelCase ? useCamelCaseResolver : noCamelCaseResolver;
            }
            else
            {
                setting.ContractResolver = noCamelCaseResolver;
            }
            return setting;
        }
    }
}