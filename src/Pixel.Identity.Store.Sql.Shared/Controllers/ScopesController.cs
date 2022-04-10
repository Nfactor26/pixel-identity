using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using Pixel.Identity.Shared;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;

namespace Pixel.Identity.Store.Sql.Shared.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.CanManageScopes)]
    public class ScopesController : Core.Controllers.ScopesController
    {
        private readonly IOpenIddictApplicationManager applicationManager;

        public ScopesController(IMapper mapper, IOpenIddictScopeManager scopeManager,
            IOpenIddictApplicationManager applicationManager) : base(mapper, scopeManager)
        {
            this.applicationManager = applicationManager;
        }

        ///<inheritdoc/>
        [HttpGet()]
        public override async Task<PagedList<ScopeViewModel>> GetAll([FromQuery] GetScopesRequest request)
        {
            List<ScopeViewModel> scopeDescriptors = new List<ScopeViewModel>();

            Func<IQueryable<object>, IQueryable<object>> query = (scopes) => scopes.Where(s => string.IsNullOrEmpty(request.ScopesFilter) ? true :
                (s as OpenIddictEntityFrameworkCoreScope).DisplayName.Contains(request.ScopesFilter)
                || (s as OpenIddictEntityFrameworkCoreScope).Name.Contains(request.ScopesFilter))
                .Skip(request.Skip).Take(request.Take);

            long count = await this.scopeManager.CountAsync<object>(query, CancellationToken.None);
            await foreach (var scope in this.scopeManager.ListAsync<object>(query, CancellationToken.None))
            {
                var scopeDescriptor = mapper.Map<ScopeViewModel>(scope);
                scopeDescriptors.Add(scopeDescriptor);
            }

            return new PagedList<ScopeViewModel>()
            {
                Items = scopeDescriptors,
                ItemsCount = (int)count,
                CurrentPage = request.CurrentPage,
                PageCount = request.PageSize
            };
        }

        ///<inheritdoc/>
        [HttpDelete("{scopeId}")]
        public override async Task<IActionResult> Delete(string scopeId)
        {
            try
            {
                var result = await this.scopeManager.FindByIdAsync(scopeId);
                var scope = mapper.Map<ScopeViewModel>(result);
                if (scope != null)
                {
                    Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>> query
                       = (apps) => apps.Where(app => (app as OpenIddictEntityFrameworkCoreApplication)
                         .Permissions.Contains(scope.Name))
                         .Select(s => s as OpenIddictEntityFrameworkCoreApplication);
                    long count = await this.applicationManager.CountAsync(query, CancellationToken.None);
                    if (count > 0)
                    {
                        return BadRequest(new BadRequestResponse(new[] { $"Scope is in use by {count} applications." }));
                    }
                    await this.scopeManager.DeleteAsync(result, CancellationToken.None);
                    return Ok();
                }
                return NotFound(new NotFoundResponse($"Scope with id : {scopeId}  doesn't exist"));
            }
            catch (Exception ex)
            {
                return BadRequest(new BadRequestResponse(new[] { ex.Message }));
            }
        }
    }
}
