namespace Pixel.Identity.Core.Plugins
{
    public interface IServicePlugin
    {
        void ConfigureService(IServiceCollection services);
    }
}
