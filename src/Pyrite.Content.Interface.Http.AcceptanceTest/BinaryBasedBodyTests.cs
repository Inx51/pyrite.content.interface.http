using Bogus;
using FluentAssertions;
using Flurl.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.Content.Interface.Http.AcceptanceTest.Services;
using Pyrite.Content.Interface.Http.Test.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pyrite.Content.Interface.Http.AcceptanceTest
{
    [TestClass, TestCategory(TestCategories.Acceptance)]
    public class BinaryBasedBodyTests
    {
        private AcceptanceTestService _acceptanceTestService;
        private static List<byte[]> _imagesCache = new List<byte[]>();

        public BinaryBasedBodyTests()
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

            var byteArrayContent = await GetByteArrayContentAsync();

            //Act
            var postResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                                .PostAsync(byteArrayContent);

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

            var byteArrayContent = await GetByteArrayContentAsync();

            //Act
            await this._acceptanceTestService.Request(resourceUrlSegments)
                                             .PostAsync(byteArrayContent);

            byteArrayContent = await GetByteArrayContentAsync(true);

            var postResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                                .PostAsync(byteArrayContent);

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

            var byteArrayContent = await GetByteArrayContentAsync();

            //Act
            await this._acceptanceTestService.Request(resourceUrlSegments)
                                             .PostAsync(byteArrayContent);

            var getResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                               .GetAsync();

            var responseByteArray = getResponse.GetBytesAsync();

            //Assert
            getResponse.StatusCode.Should().Be(200);
            responseByteArray.Should().Equals(await byteArrayContent.ReadAsByteArrayAsync());
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

            var byteArrayContent = await GetByteArrayContentAsync(true);

            //Act
            await this._acceptanceTestService.Request(resourceUrlSegments)
                                             .PostAsync(byteArrayContent);

            byteArrayContent = await GetByteArrayContentAsync();

            var response = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                            .PutAsync(byteArrayContent);

            var getResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                               .GetAsync();

            var responseByteArray = getResponse.GetBytesAsync();

            //Assert
            response.StatusCode.Should().Be(204);
            responseByteArray.Should().Equals(await byteArrayContent.ReadAsByteArrayAsync());
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

            var byteArrayContent = await GetByteArrayContentAsync(true);

            //Act
            await this._acceptanceTestService.Request(resourceUrlSegments)
                                             .PostAsync(byteArrayContent);

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

            var byteArrayContent = await GetByteArrayContentAsync(true);

            //Act
            var patchResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                                 .PatchAsync(byteArrayContent);

            //Assert
            patchResponse.StatusCode.Should().Be(405);
        }

        private async Task<ByteArrayContent> GetByteArrayContentAsync(bool reuseRandom = false)
        {
            byte[] byteArray;
            if (reuseRandom && _imagesCache.Any())
                byteArray = _imagesCache[new Randomizer().Number(0, _imagesCache.Count - 1)];
            else
            {
                byteArray = await new Bogus.DataSets.Images().LoremPixelUrl().GetBytesAsync();
                _imagesCache.Add(byteArray);
            }

            return new ByteArrayContent(byteArray);
        }
    }
}