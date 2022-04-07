namespace Pixel.Identity.Store.Sql.Shared.Stores;

public class RoleStore : RoleStore<ApplicationRole, ApplicationDbContext, Guid, IdentityUserRole<Guid>, IdentityRoleClaim>
{
    private DbSet<IdentityRoleClaim> RoleClaims { get { return Context.Set<IdentityRoleClaim>(); } }

    public RoleStore(ApplicationDbContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }

    public async override Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return await RoleClaims.Where(rc => rc.RoleId.Equals(role.Id)).Select(c => c.ToClaim()).ToListAsync(cancellationToken);
    }

    protected override IdentityRoleClaim CreateRoleClaim(ApplicationRole role, Claim claim)
    {
        var roleClaim = base.CreateRoleClaim(role, claim);
        roleClaim.InitializeFromClaim(claim);
        return roleClaim;
    }
}
