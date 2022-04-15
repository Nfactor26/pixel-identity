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
    /// Api endpoint for managing application configurations in OpenIddict
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.CanManageApplications)]
    public abstract class ApplicationsController : ControllerBase
    {
        protected readonly IMapper mapper;
        protected readonly IOpenIddictApplicationManager applicationManager;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="applicationManager"></param>
        public ApplicationsController(IMapper mapper, IOpenIddictApplicationManager applicationManager)
        {
            this.mapper = mapper;
            this.applicationManager = applicationManager;
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
            applicationDescriptor.ClientSecret = string.Empty;
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
                return CreatedAtAction(nameof(Get), new { clientId = applicationDescriptor.ClientId }, result);
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
                    //No new secret to update. Populate existing on descriptor before updating
                    if (applicationDescriptor.IsConfidentialClient && string.IsNullOrEmpty(applicationDescriptor.ClientSecret))
                    {
                        var descriptorFromExisting = new OpenIddictApplicationDescriptor();
                        await applicationManager.PopulateAsync(descriptorFromExisting, existing);
                        openIdApplicationDescriptor.ClientSecret = descriptorFromExisting.ClientSecret;
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
                return Ok();
            }
            return NotFound(new NotFoundResponse($"Failed to find application with Id : {clientId}"));
        }
    }
}
