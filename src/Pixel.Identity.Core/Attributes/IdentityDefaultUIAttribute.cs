namespace Pixel.Identity.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class IdentityDefaultUIAttribute : Attribute
    {
        public IdentityDefaultUIAttribute(Type implementationTemplate)
        {
            Template = implementationTemplate;
        }

        public Type Template { get; }
    }
}
