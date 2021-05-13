using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pyrite.Content.Abstractions.Interfaces.Repositories;
using Pyrite.Content.Interface.Http.HttpMethodStrategies;
using Pyrite.Content.Interface.Http.Test.Constants;
using System.Threading.Tasks;

namespace Pyrite.Content.HttpMethodStrategies.UnitTest
{
    [TestClass, TestCategory(TestCategories.UnitTest)]
    public class DeleteHttpMethodStrategyTests
    {
        [TestMethod]
        public async Task DeleteHttpMethodStrategy_ExecuteAsync_ShouldRespondWith204AndResourceExists()
        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "DELETE";

            var resourceRepositoryMock = new Mock<IResourceRepository>();
            resourceRepositoryMock.Setup(m => m.ExistsAsync(httpContext.Request.Path))
                                  .Returns(Task.FromResult(true));
            var deleteHttpMethodStrategy = new DeleteHttpMethodStrategy(resourceRepositoryMock.Object);

            //act
            await deleteHttpMethodStrategy.ExecuteAsync(httpContext);

            //assert
            Assert.AreEqual(204, httpContext.Response.StatusCode);
            resourceRepositoryMock.Verify
            (
                v =>
                v.DeleteAsync
                (
                    httpContext.Request.Path
                ),
                Times.Once
            );
        }

        [TestMethod]
        public async Task DeleteHttpMethodStrategy_ExecuteAsync_ShouldRespondWith404AndResourceDoesntExists()
        {
            //arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("test.pyrite");
            httpContext.Request.Path = new PathString("/jsonresources/person/1");
            httpContext.Request.Method = "DELETE";

            var resourceRepositoryMock = new Mock<IResourceRepository>();
            resourceRepositoryMock.Setup(m => m.ExistsAsync(httpContext.Request.Path))
                                  .Returns(Task.FromResult(false));
            var deleteHttpMethodStrategy = new DeleteHttpMethodStrategy(resourceRepositoryMock.Object);

            //act
            await deleteHttpMethodStrategy.ExecuteAsync(httpContext);

            //assert
            Assert.AreEqual(404, httpContext.Response.StatusCode);
            resourceRepositoryMock.Verify
            (
                v =>
                v.DeleteAsync
                (
                    httpContext.Request.Path
                ),
                Times.Never
            );
        }
    }
}