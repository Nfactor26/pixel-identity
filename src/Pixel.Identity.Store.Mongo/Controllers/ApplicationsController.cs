using AutoMapper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;
using System.Diagnostics;

namespace Pixel.Identity.Store.Mongo.Controllers;

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

        Func<IQueryable<object>, IQueryable<OpenIddictMongoDbApplication>> query;
        long count = 0;
        if (string.IsNullOrEmpty(request.ApplicationFilter))
        {
            query = (apps) => apps.Where(app => true).Skip(request.Skip).Take(request.Take).Select(s => s as OpenIddictMongoDbApplication);
            count = await this.applicationManager.CountAsync();
        }
        else
        {
            query = (apps) => apps.Where(app => (app as OpenIddictMongoDbApplication).DisplayName.Contains(request.ApplicationFilter)
            || (app as OpenIddictMongoDbApplication).ClientId.Contains(request.ApplicationFilter))
               .Skip(request.Skip).Take(request.Take).Select(s => s as OpenIddictMongoDbApplication);
            count = await this.applicationManager.CountAsync(query, CancellationToken.None);
        }

        await foreach (var app in this.applicationManager.ListAsync(query, CancellationToken.None))
        {
            var applicationDescriptor = mapper.Map<ApplicationViewModel>(app);            
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
