using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using Pixel.Identity.Shared;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;

namespace Pixel.Identity.Core.Controllers
{
    /// <summary>
    /// Api endpoint for managing application configurations in OpenIddict
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.CanManageApplications)]
    public abstract class ApplicationsController : ControllerBase
    {
        protected readonly IMapper mapper;
        protected readonly IOpenIddictApplicationManager applicationManager;
        protected readonly ICorsPolicyProvider corsPolicyProvider;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="applicationManager"></param>
        public ApplicationsController(IMapper mapper, IOpenIddictApplicationManager applicationManager, ICorsPolicyProvider corsPolicyProvider)
        {
            this.mapper = mapper;
            this.applicationManager = applicationManager;
            this.corsPolicyProvider = corsPolicyProvider;
        }
     
        /// <summary>
        /// Get all the available <see cref="OpenIddictApplicationDescriptor"/> that match request filter criteria
        /// </summary>
        /// <param name="request"><see cref="GetApplicationsRequest"/></param>
        /// <returns>Collection of <see cref="ApplicationViewModel"/></returns>
        [HttpGet()]
        public abstract Task<PagedList<ApplicationViewModel>> GetAll([FromQuery] GetApplicationsRequest request);

        /// <summary>
        /// Get <see cref="ApplicationViewModel"/> matching clientId
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet("{clientId}")]
        public async Task<ApplicationViewModel> Get(string clientId)
        {
            var app = await applicationManager.FindByClientIdAsync(clientId, CancellationToken.None);
            var applicationDescriptor = mapper.Map<ApplicationViewModel>(app);          
            return applicationDescriptor;
        }

        /// <summary>
        /// Create a new <see cref="OpenIddictApplicationDescriptor"/>
        /// </summary>
        /// <param name="applicationDescriptor"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApplicationViewModel applicationDescriptor)
        {
            if (ModelState.IsValid)
            {
                var openIdApplicationDescriptor = mapper.Map<OpenIddictApplicationDescriptor>(applicationDescriptor);
                var result = await applicationManager.CreateAsync(openIdApplicationDescriptor, CancellationToken.None);
                if(applicationDescriptor.RedirectUris.Any())
                {
                    await AllowOriginsAsync(applicationDescriptor.RedirectUris);
                }
                return CreatedAtAction(nameof(Get), new { clientId = applicationDescriptor.ClientId }, mapper.Map<ApplicationViewModel>(result));
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        /// <summary>
        /// Update details of <see cref="OpenIddictApplicationDescriptor"/>
        /// </summary>
        /// <param name="applicationDescriptor">Model carrying the changes </param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ApplicationViewModel applicationDescriptor)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(applicationDescriptor.Id))
                {
                    return BadRequest(new BadRequestResponse(new[] { "Missing Id on model." }));
                }
                var existing = await applicationManager.FindByIdAsync(applicationDescriptor.Id);
                if (existing != null)
                {
                    var openIdApplicationDescriptor = mapper.Map<OpenIddictApplicationDescriptor>(applicationDescriptor);
                    var descriptorFromExisting = new OpenIddictApplicationDescriptor();
                    await applicationManager.PopulateAsync(descriptorFromExisting, existing);
                 
                    //No new secret to update. Populate existing on descriptor before updating
                    if (applicationDescriptor.IsConfidentialClient)
                    {                     
                        if(string.IsNullOrEmpty(applicationDescriptor.ClientSecret))
                        {
                            openIdApplicationDescriptor.ClientSecret = descriptorFromExisting.ClientSecret;
                        }
                        if(string.IsNullOrEmpty(applicationDescriptor.JsonWebKeySet))
                        {
                            openIdApplicationDescriptor.JsonWebKeySet = descriptorFromExisting.JsonWebKeySet;
                        }
                    }

                    if(!openIdApplicationDescriptor.RedirectUris.SequenceEqual(descriptorFromExisting.RedirectUris))
                    {
                        await RemoveOriginsAsync(descriptorFromExisting.RedirectUris);
                        await AllowOriginsAsync(applicationDescriptor.RedirectUris);
                    }
                    
                    await applicationManager.UpdateAsync(existing, openIdApplicationDescriptor, CancellationToken.None);
                    return Ok();
                }
                return NotFound(new NotFoundResponse($"Failed to find application with Id : {applicationDescriptor.Id}"));
            }
            return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
        }

        /// <summary>
        /// Delete an application given it's clientId
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpDelete("{clientId}")]
        public async Task<IActionResult> Delete(string clientId)
        {
            var existing = await applicationManager.FindByClientIdAsync(clientId);
            if (existing != null)
            {
                await applicationManager.DeleteAsync(existing);
                var descriptor = mapper.Map<ApplicationViewModel>(existing);
                if (descriptor.RedirectUris.Any())
                {
                    await RemoveOriginsAsync(descriptor.RedirectUris);
                }
                return Ok();
            }
            return NotFound(new NotFoundResponse($"Failed to find application with Id : {clientId}"));
        }

        /// <summary>
        /// Add a uri to list of allowed origins on default cors policy
        /// </summary>
        /// <param name="origins"></param>
        /// <returns></returns>
        protected virtual async Task AllowOriginsAsync(IEnumerable<Uri> origins)
        {
            var defaultCorsPolicy = await corsPolicyProvider.GetPolicyAsync(this.HttpContext, null);         
            foreach (var uri in origins)
            {
                string origin = $"{uri.Scheme}://{uri.Authority}";
                if (!defaultCorsPolicy.Origins.Contains(origin))
                {
                    defaultCorsPolicy.Origins.Add(origin);
                }
            }
        }

        /// <summary>
        /// Remmove a uri from list of allowed origins on default cors policy
        /// </summary>
        /// <param name="origins"></param>
        /// <returns></returns>
        protected virtual async Task RemoveOriginsAsync(IEnumerable<Uri> origins)
        {
            var defaultCorsPolicy = await corsPolicyProvider.GetPolicyAsync(this.HttpContext, null);
            foreach (var uri in origins)
            {
                string origin = $"{uri.Scheme}://{uri.Authority}";
                if (defaultCorsPolicy.Origins.Contains(origin))
                {
                    defaultCorsPolicy.Origins.Remove(origin);
                }
            }
        }
    }
}
