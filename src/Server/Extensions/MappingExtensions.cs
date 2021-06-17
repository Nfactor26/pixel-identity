using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.ViewModels;

namespace Pixel.Identity.Provider.Extensions
{
    public static class MappingExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(ApplicationRole).Assembly);              
               
                cfg.CreateMap(typeof(ApplicationViewModel), typeof(OpenIddictApplicationDescriptor))
                .ForMember(nameof(OpenIddictApplicationDescriptor.DisplayNames), opt => opt.Ignore());

                cfg.CreateMap(typeof(ScopeViewModel), typeof(OpenIddictScopeDescriptor))
                .ForMember(nameof(OpenIddictScopeDescriptor.DisplayNames), opt => opt.Ignore())
                .ForMember(nameof(OpenIddictScopeDescriptor.Properties), opt => opt.Ignore())
                .ForMember(nameof(OpenIddictScopeDescriptor.Descriptions), opt => opt.Ignore());
            }
           );    
            

            #if DEBUG
            configuration.AssertConfigurationIsValid();
            #endif            
           
            var mapper = configuration.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }
    }
}
