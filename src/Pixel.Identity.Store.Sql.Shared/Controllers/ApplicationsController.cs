using AutoMapper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;

namespace Pixel.Identity.Store.Sql.Shared.Controllers;

/// <inheritdoc/>
public class ApplicationsController : Core.Controllers.ApplicationsController
{
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="applicationManager"></param>
    public ApplicationsController(IMapper mapper, IOpenIddictApplicationManager applicationManager, ICorsPolicyProvider corsPolicyProvider)
        : base(mapper, applicationManager, corsPolicyProvider)
    {
    }

    /// <inheritdoc/>
    [HttpGet]
    public override async Task<PagedList<ApplicationViewModel>> GetAll([FromQuery] GetApplicationsRequest request)
    {
        List<ApplicationViewModel> applicationDescriptors = new List<ApplicationViewModel>();

        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>> query;
        if (string.IsNullOrEmpty(request.ApplicationFilter))
        {
            query = (apps) => apps.Where(app => true).Skip(request.Skip).Take(request.Take)
            .Select(s => s as OpenIddictEntityFrameworkCoreApplication).OrderBy(s => s.ClientId);
        }
        else
        {
            query = (apps) => apps.Where(app => (app as OpenIddictEntityFrameworkCoreApplication).DisplayName.Contains(request.ApplicationFilter)
            || (app as OpenIddictEntityFrameworkCoreApplication).ClientId.Contains(request.ApplicationFilter))
               .Skip(request.Skip).Take(request.Take).Select(s => s as OpenIddictEntityFrameworkCoreApplication).OrderBy(s => s.ClientId);
        }

        long count = string.IsNullOrEmpty(request.ApplicationFilter) ? await this.applicationManager.CountAsync() :
           await this.applicationManager.CountAsync<object>(query, CancellationToken.None);
       
        await foreach (var app in this.applicationManager.ListAsync(query, CancellationToken.None))
        {
            var applicationDescriptor = mapper.Map<ApplicationViewModel>(app);
            applicationDescriptor.ClientSecret = string.Empty;
            applicationDescriptors.Add(applicationDescriptor);
        }

        return new PagedList<ApplicationViewModel>()
        {
            Items = applicationDescriptors,
            ItemsCount = (int)count,
            CurrentPage = request.CurrentPage,
            PageCount = request.CurrentPage
        };
    }
}
