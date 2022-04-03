using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Reflection;

namespace Pixel.Identity.Core.Conventions
{
    public class IdentityPageModelConvention<TUser, TKey> : IPageApplicationModelConvention
        where TUser : class
        where TKey : IEquatable<TKey>
    {
        public void Apply(PageApplicationModel model)
        {
            var defaultUIAttribute = model.ModelType.GetCustomAttribute<IdentityDefaultUIAttribute>();
            if (defaultUIAttribute == null)
            {
                return;
            }

            ValidateTemplate(defaultUIAttribute.Template);
            var templateInstance = defaultUIAttribute.Template.MakeGenericType(typeof(TUser), typeof(TKey));
            model.ModelType = templateInstance.GetTypeInfo();
        }

        private static void ValidateTemplate(Type template)
        {
            if (template.IsAbstract || !template.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException("Implementation type can't be abstract or non generic.");
            }
            var genericArguments = template.GetGenericArguments();
            if (genericArguments.Length != 2)
            {
                throw new InvalidOperationException("Implementation type contains wrong generic arity.");
            }
        }
    }
}
