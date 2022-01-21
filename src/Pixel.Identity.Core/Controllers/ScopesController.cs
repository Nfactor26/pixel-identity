using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using Pixel.Identity.Shared;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;

namespace Pixel.Identity.Core.Controllers
{
    /// <summary>
    /// Api endpoint for managing scopes in OpenIddict
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.CanManageScopes)]
    public abstract class ScopesController : ControllerBase
    {
        protected readonly IMapper mapper;
        protected readonly IOpenIddictScopeManager scopeManager;

        public ScopesController(IMapper mapper, IOpenIddictScopeManager scopeManager)
        {
            this.mapper = mapper;
            this.scopeManager = scopeManager;
        }

        /// <summary>
        /// Get all the scpoes matching request criteria
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet()]
        public abstract Task<PagedList<ScopeViewModel>> GetAll([FromQuery] GetScopesRequest request);
      

        [HttpGet("id/{id}")]
        public async Task<ScopeViewModel> Get(string id)
        {
            var result = await scopeManager.FindByIdAsync(id, CancellationToken.None);
            var scope = mapper.Map<ScopeViewModel>(result);               
            return scope;
        }

        [HttpPost()]
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

        [HttpPut()]
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

        /// <summary>
        /// Delete a scope with given Id if it is not in active use by any application
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{scopeId}")]
        public abstract Task<IActionResult> Delete(string scopeId);
    }
}
