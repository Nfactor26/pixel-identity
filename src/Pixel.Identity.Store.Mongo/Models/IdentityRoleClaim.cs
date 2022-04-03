using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace Pixel.Identity.Store.Mongo.Models
{
    /// <summary>
    /// IdentityRoleClaim with <see cref="ObjectId"/> as the Identifier type
    /// </summary>
    public class IdentityRoleClaim : IdentityRoleClaim<ObjectId>
    {       
       
    }
}
