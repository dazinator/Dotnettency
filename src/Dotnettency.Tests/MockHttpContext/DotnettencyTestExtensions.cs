using Dotnettency.Tests;
using Moq;
using System;

namespace Dotnettency
{
    public static class DotnettencyTestExtensions
    {

        public static MultitenancyOptionsBuilder<TTenant> SetMockHttpContextProvider<TTenant>(this MultitenancyOptionsBuilder<TTenant> builder, Uri uri, Action<TestHttpContextProvider> configureProvider = null)
       where TTenant : class
        {

            var request = new Mock<RequestBase>(MockBehavior.Strict);
            request.Setup(r => r.GetUri()).Returns(uri);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(c => c.Request).Returns(request.Object);

            var testHttpContextProvider = new TestHttpContextProvider(context.Object);
            configureProvider?.Invoke(testHttpContextProvider);

            builder.SetHttpContextProvider(testHttpContextProvider);          
            return builder;
        }

        public static MultitenancyOptionsBuilder<TTenant> SetMockHttpContextProvider<TTenant>(this MultitenancyOptionsBuilder<TTenant> builder, Func<Uri> getUri, Action<TestHttpContextProvider> configureProvider = null)
      where TTenant : class
        {

            var request = new Mock<RequestBase>(MockBehavior.Strict);
            request.Setup(r => r.GetUri()).Returns(getUri);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(c => c.Request).Returns(request.Object);

            var testHttpContextProvider = new TestHttpContextProvider(context.Object);
            configureProvider?.Invoke(testHttpContextProvider);

            builder.SetHttpContextProvider(testHttpContextProvider);
            return builder;
        }



    }
}




