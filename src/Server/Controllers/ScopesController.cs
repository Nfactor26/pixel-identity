using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;
using Pixel.Identity.Provider.Extensions;
using Pixel.Identity.Shared;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pixel.Identity.Provider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.IsAdmin)]
    public class ScopesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IOpenIddictScopeManager scopeManager;

        public ScopesController(IMapper mapper, IOpenIddictScopeManager scopeManager)
        {
            this.mapper = mapper;
            this.scopeManager = scopeManager;
        }

        [HttpGet()]
        public async Task<IEnumerable<ScopeViewModel>> GetAll()
        {
            List<ScopeViewModel> scopeDescriptors = new List<ScopeViewModel>();
            await foreach (var scope in this.scopeManager.ListAsync(null, null, CancellationToken.None))
            {
                var scopeDescriptor = mapper.Map<ScopeViewModel>(scope);
                if (scope is OpenIddictMongoDbScope mongoScope)
                {
                    scopeDescriptor.Id = mongoScope.Id.ToString();
                }
                scopeDescriptors.Add(scopeDescriptor);
            }
            return scopeDescriptors;
        }

        [HttpGet("id/{id}")]
        public async Task<ScopeViewModel> Get(string id)
        {
            var result = await scopeManager.FindByIdAsync(id, CancellationToken.None);
            var scope = mapper.Map<ScopeViewModel>(result);        
            if (result is OpenIddictMongoDbScope mongoScope)
            {
                scope.Id = mongoScope.Id.ToString();
            }
            return scope;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ScopeViewModel scope)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var scopeDescriptor = mapper.Map<OpenIddictScopeDescriptor>(scope);
                    await scopeManager.CreateAsync(scopeDescriptor, CancellationToken.None);
                    return Ok(new OkResponse(""));
                }
                return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemResponse(ex.Message));
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] ScopeViewModel scope)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(scope.Id))
                    {
                        return BadRequest(new BadRequestResponse(new[] { "Missing Id on scope." }));
                    }
                    var existing = await this.scopeManager.FindByIdAsync(scope.Id);
                    if (existing != null)
                    {
                        var openIdScopeDescriptor = mapper.Map<OpenIddictScopeDescriptor>(scope);
                        await this.scopeManager.UpdateAsync(existing, openIdScopeDescriptor, CancellationToken.None);
                        return Ok();
                    }
                    return NotFound(new NotFoundResponse($"Failed to find scope with Id : {scope.Id}"));
                }
                return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemResponse(ex.Message));
            }
        }
    }
}
