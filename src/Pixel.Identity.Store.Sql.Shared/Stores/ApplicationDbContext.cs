namespace Pixel.Identity.Store.Sql.Shared.Stores;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim, IdentityUserToken<Guid>>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
}
