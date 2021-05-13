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
    public class PostHttpMethodStrategyTests
    {
        [TestMethod]
        public async Task PostHttpMethodStrategy_ExecuteAsync_ShouldCreateResourceAndRespondWith201AndLocationHeader()

        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "POST";
            httpContext.Request.Headers.Add("Content-Type", "application/json");
            var bodyJson = "{\"prop1\":\"Hello, world!\"}";
            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson);
            httpContext.Request.Body = new MemoryStream(bodyBytes);
            var resourceRepositoryMock = new Mock<IResourceRepository>();
            var postHttpMethodStrategy = new PostHttpMethodStrategy(resourceRepositoryMock.Object);

            //act
            await postHttpMethodStrategy.ExecuteAsync(httpContext);

            //assert
            Assert.AreEqual(201, httpContext.Response.StatusCode);
            Assert.AreEqual($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}", httpContext.Response.Headers["Location"].ToString());
            resourceRepositoryMock.Verify
            (
                v =>
                v.CreateAsync(It.IsAny<Resource>()),
                Times.Once
            );
        }

        [TestMethod]
        public async Task PostHttpMethodStrategy_ExecuteAsync_ShouldRespondWith409AndResourceAlreadyExists()
        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "POST";
            httpContext.Request.Headers.Add("Content-Type", "application/json");
            var bodyJson = "{\"prop1\":\"Hello, world!\"}";
            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson);
            httpContext.Request.Body = new MemoryStream(bodyBytes);

            var resourceRepositoryMock = new Mock<IResourceRepository>();
            resourceRepositoryMock.Setup(m => m.ExistsAsync(httpContext.Request.Path))
                                  .Returns(Task.FromResult(true));
            var postHttpMethodStrategy = new PostHttpMethodStrategy(resourceRepositoryMock.Object);

            //act
            await postHttpMethodStrategy.ExecuteAsync(httpContext);

            //assert
            Assert.AreEqual(409, httpContext.Response.StatusCode);
            resourceRepositoryMock.Verify
            (
                v =>
                v.CreateAsync
                (
                    new Resource
                    (
                        httpContext.Request.Path,
                        It.IsAny<IEnumerable<KeyValuePair<string, StringValues>>>(),
                        It.IsAny<Stream>()
                    )
                ),
                Times.Never
            );
        }
    }
}