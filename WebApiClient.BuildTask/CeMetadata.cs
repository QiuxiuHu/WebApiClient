﻿using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebApiClient.BuildTask
{
    /// <summary>
    /// 表示cecil元数据抽象
    /// </summary>
    abstract class CeMetadata
    {
        /// <summary>
        /// 所在程序集
        /// </summary>
        private readonly ModuleDefinition module;

        /// <summary>
        /// 所有已知类型
        /// </summary>
        private readonly TypeDefinition[] knowTypes;

        /// <summary>
        /// 获取系统类型
        /// </summary>
        public TypeSystem TypeSystem { get; private set; }

        /// <summary>
        /// cecil元数据抽象
        /// </summary>
        /// <param name="module">所在程序集</param>
        public CeMetadata(CeAssembly assembly)
        {
            this.module = assembly.MainMdule;
            this.knowTypes = assembly.KnowTypes;
            this.TypeSystem = this.module.TypeSystem;
        }

        /// <summary>
        /// 返回导入外部类型后的类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        protected TypeReference ImportType<T>()
        {
            var type = typeof(T);
            if (type.IsArray == true)
            {
                throw new NotSupportedException("不支持数组类型");
            }

            var knowType = this.knowTypes.FirstOrDefault(item => item.FullName == type.FullName);
            if (knowType == null)
            {
                // 本程序集的类型不作直接导入
                if (this.IsThisAssemblyType(type) == true)
                {
                    throw new TypeLoadException($"找不到类型：{type.FullName}");
                }
                return this.module.ImportReference(type);
            }
            else
            {
                return this.module.ImportReference(knowType);
            }
        }


        /// <summary>
        /// 返回指定类型是在本程序集范围内
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private bool IsThisAssemblyType(Type type)
        {
#if NETCOREAPP1_1
            return type.GetTypeInfo().Assembly == this.GetType().GetTypeInfo().Assembly;
#else
            return type.Assembly == this.GetType().Assembly;
#endif
        }


        /// <summary>
        /// 返回导入外部类型的指定方法后的方法
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        protected MethodReference ImportMethod<T>(string methodName)
        {
            return this.ImportMethod<T>(item => item.Name == methodName);
        }

        /// <summary>
        /// 返回导入外部类型的指定方法后的方法
        /// </summary>
        /// <param name="filter">方法过滤器</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        protected MethodReference ImportMethod<T>(Func<MethodDefinition, bool> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var method = this.ImportType<T>().Resolve().Methods.FirstOrDefault(filter);
            if (method == null)
            {
                throw new ArgumentException("无法找到指定的方法");
            }
            return this.module.ImportReference(method);
        }

        /// <summary>
        /// 比较两类型类型是一样
        /// </summary>
        /// <param name="source">类型</param>
        /// <param name="target">目标类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        protected bool IsTypeEquals(TypeReference source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return source.FullName == target.FullName;
        }

        /// <summary>
        /// 返回方法的完整名称
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns></returns>
        protected string GetMethodFullName(MethodReference method)
        {
            var builder = new StringBuilder();
            foreach (var p in method.Parameters)
            {
                if (builder.Length > 0)
                {
                    builder.Append(",");
                }
                builder.Append(GetTypeName(p.ParameterType));
            }
            var insert = $"{GetTypeName(method.ReturnType)} {method.Name}(";
            return builder.Insert(0, insert).Append(")").ToString();
        }

        /// <summary>
        /// 返回类型不含namespace的名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private static string GetTypeName(TypeReference type)
        {
            if (type.IsGenericInstance == false)
            {
                return type.Name;
            }

            var builder = new StringBuilder();
            var parameters = ((GenericInstanceType)type).GenericArguments;
            foreach (var p in parameters)
            {
                if (builder.Length > 0)
                {
                    builder.Append(",");
                }
                builder.Append(GetTypeName(p));
            }

            return builder.Insert(0, $"{type.Name}<").Append(">").ToString();
        }
    }
}
