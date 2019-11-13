﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;


namespace WebApiClient.Test.Attributes.ParameterAttributes
{
    public class HeadersAttributeTest
    {
        public interface IMyApi : IDisposable
        {
            ITask<HttpResponseMessage> PostAsync(object headers);
        }

        [Fact]
        public async Task IApiParameterAttributeTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync")));

            var parameter = context.ApiActionDescriptor.Parameters[0].Clone(new
            {
                @class = 123,
                User_Agent = "WebApiClient"
            });

            var attr = new HeadersAttribute();
            await attr.BeforeRequestAsync(context, parameter);

            context.RequestMessage.Headers.TryGetValues("User-Agent", out IEnumerable<string> values);
            Assert.Equal("WebApiClient", values.FirstOrDefault());

            context.RequestMessage.Headers.TryGetValues("class", out IEnumerable<string> cValues);
            Assert.Equal("123", cValues.FirstOrDefault());
        }
    }
}
