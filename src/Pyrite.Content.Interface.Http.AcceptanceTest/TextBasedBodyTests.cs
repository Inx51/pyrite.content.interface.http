using FluentAssertions;
using Flurl.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.Content.Interface.Http.AcceptanceTest.Services;
using Pyrite.Content.Interface.Http.Test.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pyrite.Content.Interface.Http.AcceptanceTest
{
    [TestClass, TestCategory(TestCategories.Acceptance)]
    public class TextBasedBodyTests
    {
        private AcceptanceTestService _acceptanceTestService;

        public TextBasedBodyTests()
        {
            this._acceptanceTestService = new AcceptanceTestService();
        }

        [TestMethod]
        public async Task ShouldBeAbleToCreateAResource()
        {
            //Arrange
            string[] resourceUrlSegments =
            {
                "resource",
                Guid.NewGuid().ToString()
            };

            var jsonContent = new Dictionary<string, object>
            {
                ["hello"] = "world"
            };

            //Act
            var postResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                            .PostJsonAsync(jsonContent);

            //Assert
            postResponse.StatusCode.Should().Be(201);
            postResponse.ResponseMessage.Headers.Location.Should().NotBeNull().And
                                                         .NotBe(string.Empty).And
                                                         .Be(postResponse.ResponseMessage.RequestMessage.RequestUri.AbsoluteUri);
        }

        [TestMethod]
        public async Task ShouldNotBeAbleToCreateAResourceIfOneAlreadyExists()
        {
            //Arrange
            string[] resourceUrlSegments =
            {
                "resource",
                Guid.NewGuid().ToString()
            };

            var jsonContent = new Dictionary<string, object>
            {
                ["hello"] = "world"
            };

            //Act
            await this._acceptanceTestService.Request(resourceUrlSegments)
                                                            .PostJsonAsync(jsonContent);

            jsonContent = new Dictionary<string, object>
            {
                ["something"] = "else"
            };
            var postResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                                .PostJsonAsync(jsonContent);

            //Assert
            postResponse.StatusCode.Should().Be(409);
        }

        [TestMethod]
        public async Task ShouldBeAbleToGetAResource()
        {
            //Arrange
            string[] resourceUrlSegments =
            {
                "resource",
                Guid.NewGuid().ToString()
            };

            var jsonContent = new Dictionary<string, object>
            {
                ["hello"] = "world"
            };

            //Act
            await this._acceptanceTestService.Request(resourceUrlSegments)
                                             .PostJsonAsync(jsonContent);

            var getResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                            .GetAsync();

            var responseJson = getResponse.GetJsonAsync<Dictionary<string, object>>();

            //Assert
            getResponse.StatusCode.Should().Be(200);
            responseJson.Should().Equals(jsonContent);
        }

        [TestMethod]
        public async Task ShouldNotBeAbleToGetAResourceThatDoesntExists()
        {
            //Arrange
            string[] resourceUrlSegments =
            {
                "resource",
                Guid.NewGuid().ToString()
            };

            //Act
            var getResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                               .GetAsync();

            //Assert
            getResponse.StatusCode.Should().Be(404);
        }

        [TestMethod]
        public async Task ShouldBeAbleToReplaceAResource()
        {
            //Arrange
            string[] resourceUrlSegments =
            {
                "resource",
                Guid.NewGuid().ToString()
            };

            var jsonContent = new Dictionary<string, object>
            {
                ["hello"] = "world"
            };

            //Act
            await this._acceptanceTestService.Request(resourceUrlSegments)
                                             .PostJsonAsync(jsonContent);

            jsonContent = new Dictionary<string, object>
            {
                ["replaced"] = true
            };
            var response = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                            .PutJsonAsync(jsonContent);

            var getResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                            .GetAsync();

            var getResponseJson = getResponse.GetJsonAsync<Dictionary<string, object>>();

            //Assert
            response.StatusCode.Should().Be(204);
            getResponseJson.Should().Equals(jsonContent);
        }

        [TestMethod]
        public async Task ShouldBeAbleToRemoveAResource()
        {
            //Arrange
            string[] resourceUrlSegments =
            {
                "resource",
                Guid.NewGuid().ToString()
            };

            var jsonContent = new Dictionary<string, object>
            {
                ["hello"] = "world"
            };

            //Act
            await this._acceptanceTestService.Request(resourceUrlSegments)
                                             .PostJsonAsync(jsonContent);

            var deleteResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                             .DeleteAsync();

            var getResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                               .GetAsync();

            //Assert
            deleteResponse.StatusCode.Should().Be(204);
            getResponse.StatusCode.Should().Be(404);
        }

        [TestMethod]
        public async Task ShouldNotBeAbleToRemoveAResourceThatDoesntExists()
        {
            //Arrange
            string[] resourceUrlSegments =
            {
                "resource",
                Guid.NewGuid().ToString()
            };

            //Act
            var deleteResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                             .DeleteAsync();

            //Assert
            deleteResponse.StatusCode.Should().Be(404);
        }

        [TestMethod]
        public async Task ShouldNotBeAbleToUseUnimplementedHttpMethods()
        {
            //Arrange
            string[] resourceUrlSegments =
            {
                "resource",
                Guid.NewGuid().ToString()
            };

            var jsonContent = new Dictionary<string, object>
            {
                ["hello"] = "world"
            };

            //Act
            var patchResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                            .PatchJsonAsync(jsonContent);

            //Assert
            patchResponse.StatusCode.Should().Be(405);
        }
    }
}