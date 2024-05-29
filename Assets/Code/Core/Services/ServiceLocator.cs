using System;
using System.Collections.Generic;

using UnityEngine.Assertions;

namespace UIPackage.Core.Services
{
    public class ServiceLocator
    {
        public static ServiceLocator Instance => _instance ??= new ServiceLocator();

        private static ServiceLocator _instance;
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public bool Contains<Type>()
        {
            return _services.ContainsKey(typeof(Type));
        }

        public Type GetService<Type>()
        {
            var serviceType = typeof(Type);
            if (!_services.TryGetValue(serviceType, out var service))
            {
                throw new Exception($"Service of type {serviceType} not found");
            }
            return (Type)service;
        }

        public void RegisterService<Type>(Type service)
        {
            var serviceType = typeof(Type);
            Assert.IsFalse(_services.ContainsKey(serviceType), $"Service of type {serviceType} already registered");
            _services.Add(serviceType, service);
        }

        public void UnregisterService<Type>()
        {
            var serviceType = typeof(Type);
            Assert.IsTrue(_services.ContainsKey(serviceType), $"Service of type {serviceType} not registered");
            _services.Remove(serviceType);
        }

        private ServiceLocator()
        {
            _services = new Dictionary<Type, object>();
        }
    }
}