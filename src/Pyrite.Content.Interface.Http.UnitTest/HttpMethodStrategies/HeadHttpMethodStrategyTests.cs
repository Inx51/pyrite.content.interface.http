using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pyrite.Content.Abstractions.Interfaces.Repositories;
using Pyrite.Content.Domain;
using Pyrite.Content.Interface.Http.HttpMethodStrategies;
using Pyrite.Content.Interface.Http.Test.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pyrite.Content.HttpMethodStrategies.UnitTest
{
    [TestClass, TestCategory(TestCategories.UnitTest)]
    public class HeadHttpMethodStrategyTests
    {
        [TestMethod]
        public async Task HeadHttpMethodStrategy_ExecuteAsync_ShouldRespondWith204WithHeadersAndResourceExists()
        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "HEAD";

            var resourceRepositoryMock = new Mock<IResourceRepository>();
            resourceRepositoryMock.Setup(m => m.ExistsAsync(httpContext.Request.Path))
                                  .Returns(Task.FromResult(true));

            var contentTypeHeaderName = "Content-Type";
            var contentTypeHeaderValue = "application/json";
            resourceRepositoryMock.Setup(m => m.GetAsync(httpContext.Request.Path, false))
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
                                                null
                                            )
                                        )
                                    );
            var headHttpMethodStrategy = new HeadHttpMethodStrategy(resourceRepositoryMock.Object);

            //act
            await headHttpMethodStrategy.ExecuteAsync(httpContext);

            //assert
            httpContext.Response.StatusCode.Should().Be(204);
            httpContext.Response.Headers[contentTypeHeaderName].ToString().Should().Be(contentTypeHeaderValue);
            resourceRepositoryMock.Verify
            (
                v =>
                v.GetAsync
                (
                    httpContext.Request.Path,
                    false
                ),
                Times.Once
            );
        }

        [TestMethod]
        public async Task HeadHttpMethodStrategy_ExecuteAsync_ShouldRespondWith404AndResourceDoesntExists()
        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "HEAD";

            var resourceRepositoryMock = new Mock<IResourceRepository>();
            resourceRepositoryMock.Setup(m => m.ExistsAsync(httpContext.Request.Path))
                                  .Returns(Task.FromResult(false));
            var headHttpMethodStrategy = new HeadHttpMethodStrategy(resourceRepositoryMock.Object);

            //act
            await headHttpMethodStrategy.ExecuteAsync(httpContext);

            //assert
            httpContext.Response.StatusCode.Should().Be(404);
            resourceRepositoryMock.Verify
            (
                v =>
                v.GetAsync
                (
                    httpContext.Request.Path,
                    false
                ),
                Times.Never
            );
        }
    }
}