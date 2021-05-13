using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pyrite.Content.Interface.Http;
using Pyrite.Content.Interface.Http.Interfaces;
using Pyrite.Content.Interface.Http.Interfaces.Factories;
using Pyrite.Content.Test.Constants;
using System.Threading.Tasks;

namespace Pyrite.Content.UnitTest
{
    [TestClass, TestCategory(TestCategories.UnitTest)]
    public class PyriteMiddlewareTests
    {
        [TestMethod]
        public async Task PyriteMiddleware_InvokeAsync_ShouldRespondWith409IfHttpMethodNotImplemented()

        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "NOTIMPLEMENTED";
            var httpMethodStrategyFactoryMock = new Mock<IHttpMethodStrategyFactory>();
            var pyriteMiddleware = new PyriteHttpMiddleware((innerContext) => Task.FromResult(0));

            //act
            await pyriteMiddleware.InvokeAsync
            (
                httpContext,
                httpMethodStrategyFactoryMock.Object
            );

            //arrange
            Assert.AreEqual(405, httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task PyriteMiddleware_InvokeAsync_ShouldRunHttpMethodStrategyIfMatchFound()

        {
            //arrange
            var httpMethod = "POST";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = httpMethod;
            var httpMethodStrategyMock = new Mock<IHttpMethodStrategy>();
            var httpMethodStrategyFactoryMock = new Mock<IHttpMethodStrategyFactory>();
            httpMethodStrategyFactoryMock.Setup(m => m.Create(httpMethod))
                                         .Returns(httpMethodStrategyMock.Object);
            var pyriteMiddleware = new PyriteHttpMiddleware((innerContext) => Task.FromResult(0));

            //act
            await pyriteMiddleware.InvokeAsync
            (
                httpContext,
                httpMethodStrategyFactoryMock.Object
            );

            //arrange
            httpMethodStrategyMock.Verify
            (
                v =>
                v.ExecuteAsync(httpContext),
                Times.Once
            );
        }
    }
}