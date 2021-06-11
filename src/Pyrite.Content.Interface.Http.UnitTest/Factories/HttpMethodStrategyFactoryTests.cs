using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pyrite.Content.Abstractions.Interfaces.Repositories;
using Pyrite.Content.Interface.Http.Factories;
using Pyrite.Content.Interface.Http.HttpMethodStrategies;
using Pyrite.Content.Interface.Http.Test.Constants;

namespace Pyrite.Content.Interface.Http.UnitTest.Factories
{
    [TestClass, TestCategory(TestCategories.UnitTest)]
    public class HttpMethodStrategyFactoryTests
    {
        [TestMethod]
        public void HttpMethodStrategyFactory_Create_ShouldReturnGetHttpMethodStrategyIfHttpMethodIsGet()
        {
            //arrange
            var resourceRepositoryMock = new Mock<IResourceRepository>();
            var httpMethodStrategyFactory = new HttpMethodStrategyFactory(resourceRepositoryMock.Object);

            //act
            var httpMethodStrategy = httpMethodStrategyFactory.Create("GET");

            //assert
            httpMethodStrategy.Should().BeOfType(typeof(GetHttpMethodStrategy));
        }

        [TestMethod]
        public void HttpMethodStrategyFactory_Create_ShouldReturnPostHttpMethodStrategyIfHttpMethodIsPost()
        {
            //arrange
            var resourceRepositoryMock = new Mock<IResourceRepository>();
            var httpMethodStrategyFactory = new HttpMethodStrategyFactory(resourceRepositoryMock.Object);

            //act
            var httpMethodStrategy = httpMethodStrategyFactory.Create("POST");

            //assert
            httpMethodStrategy.Should().BeOfType(typeof(PostHttpMethodStrategy));
        }

        [TestMethod]
        public void HttpMethodStrategyFactory_Create_ShouldReturnHeadHttpMethodStrategyIfHttpMethodIsHead()
        {
            //arrange
            var resourceRepositoryMock = new Mock<IResourceRepository>();
            var httpMethodStrategyFactory = new HttpMethodStrategyFactory(resourceRepositoryMock.Object);

            //act
            var httpMethodStrategy = httpMethodStrategyFactory.Create("HEAD");

            //assert
            httpMethodStrategy.Should().BeOfType(typeof(HeadHttpMethodStrategy));
        }

        [TestMethod]
        public void HttpMethodStrategyFactory_Create_ShouldReturnPutHttpMethodStrategyIfHttpMethodIsPut()
        {
            //arrange
            var resourceRepositoryMock = new Mock<IResourceRepository>();
            var httpMethodStrategyFactory = new HttpMethodStrategyFactory(resourceRepositoryMock.Object);

            //act
            var httpMethodStrategy = httpMethodStrategyFactory.Create("PUT");

            //assert
            httpMethodStrategy.Should().BeOfType(typeof(PutHttpMethodStrategy));
        }

        [TestMethod]
        public void HttpMethodStrategyFactory_Create_ShouldReturnDeleteHttpMethodStrategyIfHttpMethodIsDelete()
        {
            //arrange
            var resourceRepositoryMock = new Mock<IResourceRepository>();
            var httpMethodStrategyFactory = new HttpMethodStrategyFactory(resourceRepositoryMock.Object);

            //act
            var httpMethodStrategy = httpMethodStrategyFactory.Create("DELETE");

            //assert
            httpMethodStrategy.Should().BeOfType(typeof(DeleteHttpMethodStrategy));
        }

        [TestMethod]
        public void HttpMethodStrategyFactory_Create_ShouldReturnNullIfMethodNotImplemented()
        {
            //arrange
            var resourceRepositoryMock = new Mock<IResourceRepository>();
            var httpMethodStrategyFactory = new HttpMethodStrategyFactory(resourceRepositoryMock.Object);

            //act
            var httpMethodStrategy = httpMethodStrategyFactory.Create("THISISNOTANHTTPMETHOD");

            //assert
            httpMethodStrategy.Should().BeNull();
        }
    }
}