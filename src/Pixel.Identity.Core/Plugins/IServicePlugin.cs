namespace Pixel.Identity.Core.Plugins
{
    /// <summary>
    /// Contract to be implemented by a generic plugin
    /// </summary>
    public interface IServicePlugin
    {
        void ConfigureService(IServiceCollection services, IConfiguration configuration);
    }
}
