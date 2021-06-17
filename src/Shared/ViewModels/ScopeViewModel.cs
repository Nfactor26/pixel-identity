using AutoMapper;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;
using System.Collections.Generic;

namespace Pixel.Identity.Shared.ViewModels
{
    [AutoMap(typeof(OpenIddictScopeDescriptor))]
    [AutoMap(typeof(OpenIddictMongoDbScope))]
    public class ScopeViewModel
    {
        [IgnoreMap]
        public string Id { get; set; }
    
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public List<string> Resources { get; set; } = new();
    }
}
