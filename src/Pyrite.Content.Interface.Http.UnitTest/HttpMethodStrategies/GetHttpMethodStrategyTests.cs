using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pyrite.Content.Abstractions.Interfaces.Repositories;
using Pyrite.Content.Core;
using Pyrite.Content.Interface.Http.HttpMethodStrategies;
using Pyrite.Content.Interface.Http.Test.Constants;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Content.HttpMethodStrategies.UnitTest
{
    [TestClass, TestCategory(TestCategories.UnitTest)]
    public class GetHttpMethodStrategyTests
    {
        [TestMethod]
        public async Task GettHttpMethodStrategy_ExecuteAsync_ShouldRespondWith200AndResourceExists()
        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "GET";
            httpContext.Response.Body = new MemoryStream();

            var resourceRepositoryMock = new Mock<IResourceRepository>();
            resourceRepositoryMock.Setup(m => m.ExistsAsync(httpContext.Request.Path))
                                  .Returns(Task.FromResult(true));

            var bodyJson = "{\"prop1\":\"Hello, world!\"}";
            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson);
            var contentTypeHeaderName = "Content-Type";
            var contentTypeHeaderValue = "application/json";
            resourceRepositoryMock.Setup
            (
                m =>
                m.GetAsync
                (
                    httpContext.Request.Path,
                    true
                )
            )
            .Returns
            (
                Task.FromResult
                (
                    new Resource
                    (
                        httpContext.Request.Path,
                        new List<KeyValuePair<string, StringValues>>
                        {
                            new KeyValuePair<string, StringValues>
                            (
                                contentTypeHeaderName,
                                contentTypeHeaderValue
                            )
                        },
                        new MemoryStream(bodyBytes)
                    )
                )
            );
            var getHttpMethodStrategy = new GetHttpMethodStrategy(resourceRepositoryMock.Object);

            //act
            await getHttpMethodStrategy.ExecuteAsync(httpContext);

            //assert
            httpContext.Response.StatusCode.Should().Be(200);
            httpContext.Response.Headers[contentTypeHeaderName].ToString().Should().Be(contentTypeHeaderValue);
            httpContext.Response.Body.Length.Should().BeGreaterThan(0);
            resourceRepositoryMock.Verify
            (
                v =>
                v.GetAsync
                (
                    httpContext.Request.Path,
                    true
                ),
                Times.Once
            );
        }

        [TestMethod]
        public async Task GetHttpMethodStrategy_ExecuteAsync_ShouldRespondWith404AndResourceDoesntExists()
        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "GET";

            var resourceRepositoryMock = new Mock<IResourceRepository>();
            resourceRepositoryMock.Setup(m => m.ExistsAsync(httpContext.Request.Path))
                                  .Returns(Task.FromResult(false));

            var contentTypeHeaderName = "Content-Type";
            var getHttpMethodStrategy = new GetHttpMethodStrategy(resourceRepositoryMock.Object);

            //act
            await getHttpMethodStrategy.ExecuteAsync(httpContext);

            //assert
            httpContext.Response.StatusCode.Should().Be(404);
            httpContext.Response.Headers.Should().NotContainKey(contentTypeHeaderName);
            httpContext.Response.Body.Length.Should().Be(0);
            resourceRepositoryMock.Verify
            (
                v =>
                v.GetAsync
                (
                    httpContext.Request.Path,
                    true
                ),
                Times.Never
            );
        }
    }
}