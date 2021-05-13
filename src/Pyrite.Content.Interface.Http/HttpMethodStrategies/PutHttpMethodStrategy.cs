﻿using Microsoft.AspNetCore.Http;
using Pyrite.Content.Abstractions.Interfaces.Repositories;
using Pyrite.Content.Core;
using Pyrite.Content.Interface.Http.Interfaces;
using System.Threading.Tasks;

namespace Pyrite.Content.Interface.Http.HttpMethodStrategies
{
    public class PutHttpMethodStrategy : IHttpMethodStrategy
    {
        private readonly IResourceRepository _resourceRepository;

        public PutHttpMethodStrategy(IResourceRepository resourceRepository)
        {
            this._resourceRepository = resourceRepository;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            if (await this._resourceRepository.ExistsAsync(httpContext.Request.Path))
            {
                httpContext.Response.StatusCode = 204;
                await this._resourceRepository.DeleteAsync(httpContext.Request.Path);
            }
            else
            {
                httpContext.Response.StatusCode = 201;
                httpContext.Response.Headers.Add("Location", $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}");
            }

            await this._resourceRepository.CreateAsync
            (
                new Resource
                (
                    httpContext.Request.Path,
                    httpContext.Request.Headers,
                    httpContext.Request.Body
                )
            );
        }
    }
}