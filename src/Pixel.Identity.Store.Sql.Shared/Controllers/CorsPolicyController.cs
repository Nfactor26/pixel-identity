using Microsoft.AspNetCore.Cors.Infrastructure;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Pixel.Identity.Store.Sql.Shared.Controllers;

public class CorsPolicyController : Core.Controllers.CorsPolicyController
{
    public CorsPolicyController(IConfiguration configuration, ICorsPolicyProvider corsPolicyProvider, IOpenIddictApplicationManager applicationManager)
        : base(configuration, corsPolicyProvider, applicationManager)
    {

    }

    protected override IAsyncEnumerable<object> ListApplicationsAsync()
    {
        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>> query = (apps) =>
        {
            return apps.Where(app => true).Select(s => s as OpenIddictEntityFrameworkCoreApplication);
        };
        return this.applicationManager.ListAsync(query, CancellationToken.None);
    }
}

