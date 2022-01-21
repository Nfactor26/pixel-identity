using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;
using Pixel.Identity.Shared;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;

namespace Pixel.Identity.Store.Mongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.CanManageScopes)]
    public class ScopesController : Core.Controllers.ScopesController
    {
        private readonly IOpenIddictApplicationManager applicationManager;

        public ScopesController(IMapper mapper, IOpenIddictScopeManager scopeManager ,
            IOpenIddictApplicationManager applicationManager) : base(mapper, scopeManager)
        {
            this.applicationManager = applicationManager;
        }

        ///<inheritdoc/>
        [HttpGet()]
        public override async Task<PagedList<ScopeViewModel>> GetAll([FromQuery] GetScopesRequest request)
        {           
            List<ScopeViewModel> scopeDescriptors = new List<ScopeViewModel>();

            Func<IQueryable<object>, IQueryable<OpenIddictMongoDbScope>> query;
            if (string.IsNullOrEmpty(request.ScopesFilter))
            {
                query = (scopes) => scopes.Skip(request.Skip).Take(request.Take).Select(s => s as OpenIddictMongoDbScope);
            }
            else
            {
                query = (scopes) => scopes.Where(s => (s as OpenIddictMongoDbScope).DisplayName.StartsWith(request.ScopesFilter)
                || (s as OpenIddictMongoDbScope).Name.Contains(request.ScopesFilter))
               .Take(request.Take).Skip(request.Skip).Select(s => s as OpenIddictMongoDbScope);
            }

            long count = await this.scopeManager.CountAsync(query, CancellationToken.None);
            await foreach (var scope in this.scopeManager.ListAsync(query, CancellationToken.None))
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
                    Func<IQueryable<object>, IQueryable<OpenIddictMongoDbApplication>> query
                       = (apps) => apps.Where(app => (app as OpenIddictMongoDbApplication)
                         .Permissions.Any(p => p.Contains(scope.Name)))
                         .Select(s => s as OpenIddictMongoDbApplication);                  
                    long count = await this.applicationManager.CountAsync(query, CancellationToken.None);
                    if (count > 0)
                    {
                        return BadRequest(new BadRequestResponse(new[] { $"Scope is in use by {count} applications." }));
                    }
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
