using System.Text;

namespace Pixel.Identity.Store.Sql.Shared.Models;

/// <summary>
/// IdentityRoleClaim with <see cref="Guid"/> as the Identifier type
/// </summary>
public class IdentityUserClaim : IdentityUserClaim<Guid>
{
    /// <summary>
    /// Additional properties associated with this claim
    /// </summary>
    public virtual string Properties { get; set; }

    /// <summary>
    /// constructor
    /// </summary>
    public IdentityUserClaim()
    {

    }

    /// <inheritdoc/>  
    public override Claim ToClaim()
    {
        var claim = base.ToClaim();
        foreach (var property in this.Properties?.Trim(']', '[').Split(',', StringSplitOptions.None).Select(r => r.Trim('\"')))
        {
            var keyValue = property.Split(':');
            claim.Properties.Add(keyValue[0], keyValue[1]);
        }
        return claim;
    }

    /// <inheritdoc/>  
    public override void InitializeFromClaim(Claim claim)
    {
        base.InitializeFromClaim(claim);
        StringBuilder sb = new StringBuilder();
        foreach (var (key, value) in claim.Properties)
        {

            sb.Append("\"");
            sb.Append(key);
            sb.Append(":");
            sb.Append(value);
            sb.Append("\"");
            sb.Append(',');
        }
        this.Properties = $"[{sb.ToString().Trim(',')}]";
    }
}
