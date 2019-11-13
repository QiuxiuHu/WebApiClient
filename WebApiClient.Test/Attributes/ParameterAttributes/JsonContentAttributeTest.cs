﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Attributes.HttpActionAttributes
{
    public class JsonContentAttributeTest
    {
        public interface IMyApi : IDisposable
        {
            ITask<HttpResponseMessage> PostAsync(object content);
        }

        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new TestActionContext(
                httpApi: null,
                httpApiConfig: new HttpApiConfig(),
                apiActionDescriptor: ApiActionDescriptor.Create(typeof(IMyApi).GetMethod("PostAsync")));

            context.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.RequestMessage.Method = HttpMethod.Post; 


            var parameter = context.ApiActionDescriptor.Parameters[0].Clone(new
            {
                name = "laojiu",
                birthDay = DateTime.Parse("2010-10-10")
            });

            var attr = new JsonContentAttribute();
            await ((IApiParameterAttribute)attr).BeforeRequestAsync(context, parameter);

            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            var options = context.HttpApiConfig.FormatOptions.CloneChange(attr.DateTimeFormat);
            var target = context.HttpApiConfig.JsonFormatter.Serialize(parameter.Value, options);
            Assert.True(body == target);
        }
    }
}

