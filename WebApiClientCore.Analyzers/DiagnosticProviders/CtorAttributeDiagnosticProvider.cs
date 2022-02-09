﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Analyzers.DiagnosticProviders
{
    /// <summary>
    /// 表示特性构造函数诊断器
    /// </summary>
    sealed class CtorAttributeDiagnosticProvider : HttpApiDiagnosticProvider
    {
        /// <summary>
        /// /// <summary>
        /// 获取诊断描述
        /// </summary>
        /// </summary>
        public override DiagnosticDescriptor Descriptor => Descriptors.AttributeDescriptor;

        /// <summary>
        /// 特性构造函数诊断器
        /// </summary>
        /// <param name="context">上下文</param>
        public CtorAttributeDiagnosticProvider(HttpApiContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 返回所有的报告诊断
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Diagnostic> CreateDiagnostics()
        {
            var @interface = this.Context.Interface;      
            var interfaceAttributes = this.GetInterfaceDiagnosticAttributes(@interface);
            var methodAttributes = this.Context.Methods.SelectMany(item => this.GetMethodDiagnosticAttributes(item));

            foreach (var item in interfaceAttributes.Concat(methodAttributes))
            {
                var appSyntax = item.ApplicationSyntaxReference;
                if (appSyntax == null)
                {
                    continue;
                }

                var location = appSyntax.GetSyntax().GetLocation();
                yield return this.CreateDiagnostic(location);
            }
        }


        /// <summary>
        /// 获取接口已诊断的特性
        /// </summary>
        /// <param name="interface">类型</param>
        /// <returns></returns>
        private IEnumerable<AttributeData> GetInterfaceDiagnosticAttributes(INamedTypeSymbol @interface)
        {
            foreach (var attribute in @interface.GetAttributes())
            {
                if (this.CtorAttributeIsDefind(attribute, AttributeTargets.Interface) == false)
                {
                    yield return attribute;
                }
            }
        }


        /// <summary>
        /// 获取方法已诊断的特性
        /// </summary>
        /// <param name="methodSymbol">方法</param>
        /// <returns></returns>
        private IEnumerable<AttributeData> GetMethodDiagnosticAttributes(IMethodSymbol methodSymbol)
        {
            foreach (var methodAttribute in methodSymbol.GetAttributes())
            {
                if (this.CtorAttributeIsDefind(methodAttribute, AttributeTargets.Method) == false)
                {
                    yield return methodAttribute;
                }
            }

            foreach (var parameter in methodSymbol.Parameters)
            {
                foreach (var parameterAttribute in parameter.GetAttributes())
                {
                    if (this.CtorAttributeIsDefind(parameterAttribute, AttributeTargets.Parameter) == false)
                    {
                        yield return parameterAttribute;
                    }
                }
            }
        }


        /// <summary>
        /// 获取特性声明的AttributeCtorUsageAttribute是否声明了指定目标
        /// </summary>
        /// <param name="attributeData"></param>
        /// <param name="targets">指定目标</param>
        /// <returns></returns>
        private bool CtorAttributeIsDefind(AttributeData attributeData, AttributeTargets targets)
        {
            var ctorAttr = this.Context.AttributeCtorUsageAttribute;
            if (ctorAttr == null)
            {
                return true;
            }

            if (attributeData.AttributeConstructor == null)
            {
                return true;
            }

            var ctorUsageAttribute = attributeData
                .AttributeConstructor
                .GetAttributes()
                .FirstOrDefault(item => ctorAttr.Equals(item.AttributeClass));

            if (ctorUsageAttribute == null)
            {
                return true;
            }

            var arg = ctorUsageAttribute.ConstructorArguments.FirstOrDefault();
            if (arg.IsNull == true || arg.Value == null)
            {
                return true;
            }

            var ctorTargets = (AttributeTargets)(int)arg.Value;
            return ctorTargets.HasFlag(targets);
        }
    }
}
