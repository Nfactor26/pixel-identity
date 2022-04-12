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

        public ApplicationsController(IMapper mapper, IOpenIddictApplicationManager applicationManager)
        {
            this.mapper = mapper;
            this.applicationManager = applicationManager;
        }

        [HttpGet()]
        public abstract Task<PagedList<ApplicationViewModel>> GetAll([FromQuery] GetApplicationsRequest request);
       
        [HttpGet("{clientId}")]
        public async Task<ApplicationViewModel> Get(string clientId)
        {
            var app = await applicationManager.FindByClientIdAsync(clientId, CancellationToken.None);
            var applicationDescriptor = mapper.Map<ApplicationViewModel>(app);
            applicationDescriptor.ClientSecret = string.Empty;         
            return applicationDescriptor;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApplicationViewModel applicationDescriptor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var openIdApplicationDescriptor = mapper.Map<OpenIddictApplicationDescriptor>(applicationDescriptor);
                    await applicationManager.CreateAsync(openIdApplicationDescriptor, CancellationToken.None);
                    return Ok(new OkResponse(""));
                }
                return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemResponse(ex.Message));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ApplicationViewModel applicationDescriptor)
        {
            try
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
                        if(applicationDescriptor.IsConfidentialClient && string.IsNullOrEmpty(applicationDescriptor.ClientSecret))
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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemResponse(ex.Message));
            }
        }

        /// <summary>
        /// Delete an application given it's clientId
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpDelete("{clientId}")]
        public async Task<IActionResult> Delete(string clientId)
        {
            try
            {
                var existing = await applicationManager.FindByClientIdAsync(clientId);
                if (existing != null)
                {
                    await applicationManager.DeleteAsync(existing);
                    return Ok();
                }
                return NotFound(new NotFoundResponse($"Failed to find application with Id : {clientId}"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemResponse(ex.Message));
            }
        }
    }
}
