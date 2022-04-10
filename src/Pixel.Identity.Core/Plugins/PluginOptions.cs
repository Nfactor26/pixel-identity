namespace Pixel.Identity.Core.Plugins;

/// <summary>
/// Configuration option for a collection of <see cref="Plugin"/> that can be loaded by application
/// </summary>
public class PluginOptions
{
    public const string Plugins = "Plugins";

    public List<Plugin> Collection { get; set; } = new();
    
    /// <summary>
    /// Get a plugin based on type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerable<Plugin> this[string type]
    {
        get => this.Collection.Where(p => p.Type.Equals(type));
    }
}
