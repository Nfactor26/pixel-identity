using Microsoft.AspNetCore.Identity;

namespace Pixel.Identity.Core
{
    /// <summary>
    /// It is possible to use different backends such as MongoDb or Microsoft Sql Server with Pixel Identity.
    /// IConfigurator is the extension mechanism for backend plugins to have a chance to configure the database to be used
    /// by the asp.net identity and openiddict.
    /// </summary>
    public interface IConfigurator
    {
        /// <summary>
        /// Configure Asp.Net identity for database to be used
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        IdentityBuilder ConfigureIdentity(IConfiguration configuration, IServiceCollection services);

        /// <summary>
        /// Configure OpenIdDict for database to be used
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        OpenIddictBuilder ConfigureOpenIdDictStore(IConfiguration configuration, OpenIddictBuilder builder);

        /// <summary>
        /// Configure the AutoMap for the mapping between viewmodels and db models
        /// </summary>
        /// <param name="services"></param>
        void ConfigureAutoMap(IServiceCollection services);

        /// <summary>
        /// Add any additional services
        /// </summary>
        /// <param name="services"></param>
        void AddServices(IServiceCollection services);
    }
}
