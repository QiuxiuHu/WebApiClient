﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;

namespace WebApiClientCore.Analyzers
{
    /// <summary>
    /// 表示HttpApi上下文
    /// </summary>
    sealed class HttpApiContext
    {
        private const string IHttpApiTypeName = "WebApiClientCore.IHttpApi";
        private const string IApiActionAttributeTypeName = "WebApiClientCore.IApiActionAttribute";
        private const string UriAttributeTypeName = "WebApiClientCore.Attributes.UriAttribute";
        private const string AttributeCtorUsageTypName = "WebApiClientCore.AttributeCtorUsageAttribute";

        /// <summary>
        /// 获取语法节点上下文
        /// </summary>
        public SyntaxNodeAnalysisContext SyntaxNodeContext { get; }

        /// <summary>
        /// 获取接口声明语法
        /// </summary>
        public InterfaceDeclarationSyntax? InterfaceSyntax { get; }

        /// <summary>
        /// 获取接
        /// </summary>
        public INamedTypeSymbol? @Interface { get; }

        /// <summary>
        /// 获取是否为HttpApi
        /// </summary>
        public bool IsHtttApi { get; }

        /// <summary>
        /// 获取UriAttribute的类型
        /// </summary>
        public INamedTypeSymbol? UriAttribute { get; }

        /// <summary>
        /// 获取AttributeCtorUsageAttribute的类型
        /// </summary>
        public INamedTypeSymbol? AttributeCtorUsageAttribute { get; }

        /// <summary>
        /// 获取声明的Api方法
        /// </summary>
        public IMethodSymbol[] ApiMethods { get; } = Array.Empty<IMethodSymbol>();

        /// <summary>
        /// HttpApi上下文
        /// </summary>
        /// <param name="syntaxNodeContext"></param>
        public HttpApiContext(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            this.SyntaxNodeContext = syntaxNodeContext;
            this.InterfaceSyntax = syntaxNodeContext.Node as InterfaceDeclarationSyntax;
            if (this.InterfaceSyntax == null)
            {
                return;
            }

            this.Interface = syntaxNodeContext.Compilation.GetSemanticModel(this.InterfaceSyntax.SyntaxTree).GetDeclaredSymbol(this.InterfaceSyntax);
            if (this.Interface == null)
            {
                return;
            }

            var ihttpApi = syntaxNodeContext.Compilation.GetTypeByMetadataName(IHttpApiTypeName);
            var iapiActionAttribute = syntaxNodeContext.Compilation.GetTypeByMetadataName(IApiActionAttributeTypeName);
            if (ihttpApi != null)
            {
                this.IsHtttApi = IsHttpApiInterface(this.Interface, ihttpApi, iapiActionAttribute);
            }

            this.ApiMethods = this.Interface.GetMembers().OfType<IMethodSymbol>().ToArray();
            this.UriAttribute = syntaxNodeContext.Compilation.GetTypeByMetadataName(UriAttributeTypeName);
            this.AttributeCtorUsageAttribute = syntaxNodeContext.Compilation.GetTypeByMetadataName(AttributeCtorUsageTypName);
        }

        /// <summary>
        /// 是否为http接口
        /// </summary>
        /// <param name="interface"></param>
        /// <param name="ihttpApi"></param>
        /// <param name="iapiActionAttribute"></param>
        /// <returns></returns>
        private static bool IsHttpApiInterface(INamedTypeSymbol @interface, INamedTypeSymbol ihttpApi, INamedTypeSymbol? iapiActionAttribute)
        {
            if (@interface.AllInterfaces.Contains(ihttpApi))
            {
                return true;
            }

            if (iapiActionAttribute == null)
            {
                return false;
            }

            var interfaces = @interface.AllInterfaces.Append(@interface);
            return interfaces.Any(i => HasApiActionAttribute(i, iapiActionAttribute));
        }


        /// <summary>
        /// 返回接口和其声明的方法是否包含IApiActionAttribute
        /// </summary>
        /// <param name="interface"></param>
        /// <param name="iapiActionAttribute"></param>
        /// <returns></returns>
        private static bool HasApiActionAttribute(INamedTypeSymbol @interface, INamedTypeSymbol iapiActionAttribute)
        {
            return HasAttribute(@interface, iapiActionAttribute)
                || @interface.GetMembers().Any(m => HasAttribute(m, iapiActionAttribute));
        }

        /// <summary>
        /// 返回成员是否有特性
        /// </summary>
        /// <param name="member"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static bool HasAttribute(ISymbol member, INamedTypeSymbol attribute)
        {
            foreach (var attr in member.GetAttributes())
            {
                var attrClass = attr.AttributeClass;
                if (attrClass != null && attrClass.AllInterfaces.Contains(attribute))
                {
                    return true;
                }
            }
            return false;
        }
    }
}