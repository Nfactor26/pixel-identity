using AspNetCore.Identity.MongoDbCore.Models;
using AutoMapper;
using AutoMapper.Configuration.Annotations;

namespace Pixel.Identity.Shared.ViewModels
{
    [AutoMap(typeof(MongoClaim))]
    public class ClaimViewModel
    {
        [SourceMember(nameof(MongoClaim.Type))]
        public string Type { get; set; }

        [SourceMember(nameof(MongoClaim.Value))]
        public string Value { get; set; }
       
        public ClaimViewModel()
        {

        }

        public ClaimViewModel(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
