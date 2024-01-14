using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.Store.Mongo.Models;
using System.Security.Cryptography;

namespace Pixel.Identity.Store.Mongo;

/// <summary>
/// AutoMap profile for mapping required for MongoDb models and pixel identity view models
/// </summary>
public class AutoMapProfile : Profile
{
    public AutoMapProfile()
    {
        // OpenIddict changed the OpenIddictApplicationDescriptor.Type to be
        // OpenIddictApplicationDescriptor.ClientType and then marked the original
        // Type property as Obsolete with IsError = True. We cannot use AutoMapper's
        // ForMember(a => a.Type, opt => opt.Ignore()) method because it will not compile.
        // The workaround is to use the MemberList.Source option which allows AutoMapper
        // to ignore the Type property.

        Func<ApplicationViewModel, JsonWebKeySet?> jsonWebKeySetMapper = (a) =>
        {
            if (!string.IsNullOrEmpty(a.JsonWebKeySet))
            {
                var jsonWebKey = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(GetECDsaSigningKey(a.JsonWebKeySet));
                return new JsonWebKeySet()
                {
                    Keys = { jsonWebKey }
                };
            }
            return default;
        };

        CreateMap<ApplicationViewModel, OpenIddictApplicationDescriptor>(MemberList.Source)
           .ForSourceMember(s => s.IsConfidentialClient, opt => opt.DoNotValidate())
           .ForSourceMember(s => s.Id, opt => opt.DoNotValidate())
           .ForSourceMember(s => s.JsonWebKeySet, opt => opt.DoNotValidate())
           .ForSourceMember(s => s.ClientSecret, opt => opt.DoNotValidate())
           .ForMember(d => d.JsonWebKeySet, opt => opt.MapFrom(s => jsonWebKeySetMapper(s)));
          
    
        CreateMap<OpenIddictMongoDbApplication, ApplicationViewModel>()
        .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()))
        .ForMember(d => d.IsConfidentialClient, opt => opt.Ignore())
        .ForMember(d => d.JsonWebKeySet, opt => opt.Ignore())
        .ForMember(d => d.ClientSecret, opt => opt.Ignore());

        CreateMap<ApplicationUser, UserDetailsViewModel>()
         .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()))
         .ForMember(d => d.UserRoles, opt => opt.Ignore())
         .ForMember(d => d.UserClaims, opt => opt.Ignore());

        CreateMap<ApplicationRole, UserRoleViewModel>()
        .ForMember(d => d.RoleId, opt => opt.MapFrom(s => s.Id))
        .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Name))
        .ForMember(d => d.Claims, opt => opt.Ignore());

        CreateMap<ScopeViewModel, OpenIddictScopeDescriptor>()
        .ForMember(d => d.DisplayNames, opt => opt.Ignore())
        .ForMember(d => d.Properties, opt => opt.Ignore())
        .ForMember(d => d.Descriptions, opt => opt.Ignore());

        CreateMap<OpenIddictMongoDbScope, ScopeViewModel>()
        .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()));
    }

    static ECDsaSecurityKey GetECDsaSigningKey(ReadOnlySpan<char> key)
    {
        var algorithm = ECDsa.Create();
        algorithm.ImportFromPem(key);
        return new ECDsaSecurityKey(algorithm);
    }
}
