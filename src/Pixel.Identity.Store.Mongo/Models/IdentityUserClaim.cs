using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace Pixel.Identity.Store.Mongo.Models
{
    /// <summary>
    /// IdentityUserClaim with <see cref="ObjectId"/> as the Identifier type
    /// </summary>
    public class IdentityUserClaim : IdentityUserClaim<ObjectId>
    {
    }
}
