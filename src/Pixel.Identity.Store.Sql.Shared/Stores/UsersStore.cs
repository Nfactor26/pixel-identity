namespace Pixel.Identity.Store.Sql.Shared.Stores;

public class UsersStore : UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid,
    IdentityUserClaim, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
    IdentityUserToken<Guid>, IdentityRoleClaim>
{
    public UsersStore(ApplicationDbContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }

    public override Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        return base.GetClaimsAsync(user, cancellationToken);
    }

    protected override IdentityUserClaim CreateUserClaim(ApplicationUser user, Claim claim)
    {
        var userClaim = base.CreateUserClaim(user, claim);
        userClaim.InitializeFromClaim(claim);
        return userClaim;
    }
}
