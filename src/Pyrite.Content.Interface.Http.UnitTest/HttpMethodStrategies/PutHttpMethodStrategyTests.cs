using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pyrite.Content.Abstractions.Interfaces.Repositories;
using Pyrite.Content.Core;
using Pyrite.Content.Interface.Http.HttpMethodStrategies;
using Pyrite.Content.Test.Constants;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Content.HttpMethodStrategies.UnitTest
{
    [TestClass, TestCategory(TestCategories.UnitTest)]
    public class PutHttpMethodStrategyTests
    {
        [TestMethod]
        public async Task PutHttpMethodStrategy_InvokeAsync_ShouldRespondWith204AndRepalceResourceAndResourceExists()
        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "PUT";
            httpContext.Request.Headers.Add("Content-Type", "application/json");
            var bodyJson = "{\"prop1\":\"Hello, world!\"}";
            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson);
            httpContext.Request.Body = new MemoryStream(bodyBytes);

            var resourceRepositoryMock = new Mock<IResourceRepository>();
            resourceRepositoryMock.Setup(m => m.ExistsAsync(httpContext.Request.Path))
                      .Returns(Task.FromResult(true));
            var putHttpMethodStrategy = new PutHttpMethodStrategy(resourceRepositoryMock.Object);

            //act
            await putHttpMethodStrategy.ExecuteAsync(httpContext);

            //assert
            Assert.AreEqual(204, httpContext.Response.StatusCode);
            resourceRepositoryMock.Verify
            (
                v =>
                v.DeleteAsync(httpContext.Request.Path),
                Times.Once
            );
            resourceRepositoryMock.Verify
            (
                v =>
                v.CreateAsync(It.IsAny<Resource>()),
                Times.Once
            );
        }

        [TestMethod]
        public async Task PutHttpMethodStrategy_ExecuteAsync_ShouldRespondWith201AndCreateResourceAndResourceDoesntExists()
        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "PUT";
            httpContext.Request.Headers.Add("Content-Type", "application/json");
            var bodyJson = "{\"prop1\":\"Hello, world!\"}";
            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson);
            httpContext.Request.Body = new MemoryStream(bodyBytes);

            var resourceRepositoryMock = new Mock<IResourceRepository>();
            resourceRepositoryMock.Setup(m => m.ExistsAsync(httpContext.Request.Path))
                      .Returns(Task.FromResult(false));
            var putHttpMethodStrategy = new PutHttpMethodStrategy(resourceRepositoryMock.Object);

            //act
            await putHttpMethodStrategy.ExecuteAsync(httpContext);

            //assert
            Assert.AreEqual(201, httpContext.Response.StatusCode);
            Assert.AreEqual($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}", httpContext.Response.Headers["Location"].ToString());
            //Lets make sure that we don't accidentally remove the resource..
            resourceRepositoryMock.Verify
            (
                v =>
                v.DeleteAsync(httpContext.Request.Path),
                Times.Never
            );
            resourceRepositoryMock.Verify
            (
                v =>
                v.CreateAsync(It.IsAny<Resource>()),
                Times.Once
            );
        }
    }
}