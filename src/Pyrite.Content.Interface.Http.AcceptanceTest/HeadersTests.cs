using FluentAssertions;
using Flurl.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.Content.Interface.Http.AcceptanceTest.Services;
using Pyrite.Content.Interface.Http.Constants;
using Pyrite.Content.Interface.Http.Test.Constants;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pyrite.Content.Interface.Http.AcceptanceTest
{
    [TestClass, TestCategory(TestCategories.Acceptance)]
    public class HeadersTests
    {
        private AcceptanceTestService _acceptanceTestService;

        public HeadersTests()
        {
            this._acceptanceTestService = new AcceptanceTestService();
        }

        [TestMethod]
        public async Task ShouldBeAbleToCreateAResourceWithCustomHeaders()
        {
            //Arrange
            string[] resourceUrlSegments =
            {
                "resource",
                Guid.NewGuid().ToString()
            };

            var customHeaderName = "X-Header";
            var customHeaderValue = "Hello, world!";

            var stringContent = new StringContent(string.Empty);

            //Act
            var postResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                                .WithHeader
                                                                (
                                                                    customHeaderName,
                                                                    customHeaderValue
                                                                )
                                                                .PostAsync(stringContent);

            var getResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                               .GetAsync();

            //Assert
            postResponse.StatusCode.Should().Be(201);
            postResponse.ResponseMessage.Headers.Location.Should().NotBeNull().And
                                                         .NotBe(string.Empty).And
                                                         .Be(postResponse.ResponseMessage.RequestMessage.RequestUri.AbsoluteUri);

            getResponse.StatusCode.Should().Be(200);
            string.Join(",", getResponse.ResponseMessage.Headers.GetValues(customHeaderName)).Should().Be(customHeaderValue);
        }

        [TestMethod]
        public async Task ShouldNotContainRequestHeaders()
        {
            //Arrange
            string[] resourceUrlSegments =
            {
                "resource",
                Guid.NewGuid().ToString()
            };

            var stringContent = new StringContent(string.Empty);

            var headers = new Dictionary<string, object>();
            foreach (var requestHeader in HttpHeaders.RequestHeaders)
            {
                //Cant replace Host header..
                if (requestHeader != "Host")
                    headers.Add(requestHeader, "Test");
            }

            //Act
            var postResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                                .WithHeaders(headers)
                                                                .PostAsync(stringContent);

            var getResponse = await this._acceptanceTestService.Request(resourceUrlSegments)
                                                               .GetAsync();

            //Assert
            postResponse.StatusCode.Should().Be(201);
            postResponse.ResponseMessage.Headers.Location.Should().NotBeNull().And
                                                         .NotBe(string.Empty).And
                                                         .Be(postResponse.ResponseMessage.RequestMessage.RequestUri.AbsoluteUri);

            getResponse.StatusCode.Should().Be(200);
            foreach (var header in getResponse.Headers)
            {
                HttpHeaders.RequestHeaders.Should().NotContain(header.Name);
            }
        }
    }
}