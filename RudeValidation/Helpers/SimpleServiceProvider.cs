using System;
using System.Collections.Generic;

namespace RudeValidation.Helpers
{
    public class SimpleServiceProvider : IServiceProvider
    {
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public object GetService(Type serviceType)
        {
            if (this._services.ContainsKey(serviceType))
            {
                return this._services[serviceType];
            }

            return null;
        }

        public void AddService<T>(T service)
        {
            this._services[typeof(T)] = service;
        }
    }
}
