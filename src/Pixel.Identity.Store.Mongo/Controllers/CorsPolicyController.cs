using Microsoft.AspNetCore.Cors.Infrastructure;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;

namespace Pixel.Identity.Store.Mongo.Controllers;

public class CorsPolicyController : Core.Controllers.CorsPolicyController
{
    public CorsPolicyController(IConfiguration configuration, ICorsPolicyProvider corsPolicyProvider, IOpenIddictApplicationManager applicationManager)
        : base(configuration, corsPolicyProvider, applicationManager)
    {

    }

    protected override IAsyncEnumerable<object> ListApplicationsAsync()
    {
        Func<IQueryable<object>, IQueryable<OpenIddictMongoDbApplication>> query = (apps) =>
        {
           return apps.Where(app => true).Select(s => s as OpenIddictMongoDbApplication);
        };
        return this.applicationManager.ListAsync(query, CancellationToken.None);
    }
}

