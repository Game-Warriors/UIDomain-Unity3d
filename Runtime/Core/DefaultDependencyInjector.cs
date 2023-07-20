using GameWarriors.UIDomain.Abstraction;
using System;
using System.Reflection;

namespace GameWarriors.UIDomain.Core
{
    public class DefaultDependencyInjector : IDependencyInjector
    {
        private IServiceProvider _serviceProvider;

        public DefaultDependencyInjector(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Inject(object elementBuffer)
        {
            PropertyInfo[] properties = elementBuffer.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty);
            int length = properties?.Length ?? 0;
            for (int i = 0; i < length; ++i)
            {
                Type abstractionType = properties[i].PropertyType;
                if (properties[i].CanWrite)
                {
                    object targetService = _serviceProvider.GetService(abstractionType);
                    if (targetService != null)
                    {
                        properties[i].SetValue(elementBuffer, targetService);
                    }
                }
            }
        }
    }
}
