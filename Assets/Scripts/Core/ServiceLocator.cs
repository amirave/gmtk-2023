    using System;
    using System.Collections.Generic;
    using UnityEngine;

namespace Core
{
    public class ServiceLocator : MonoBehaviour
    {
        public static ServiceLocator Instance { get; private set; }
        
        // Dictionary to store service instances
        private Dictionary<Type, object> serviceDictionary;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;

            serviceDictionary = new Dictionary<Type, object>();
        }
        
        // Register a service instance
        public void RegisterService<T>(T serviceInstance)
        {
            Type serviceType = typeof(T);
            if (serviceDictionary.ContainsKey(serviceType))
            {
                Debug.LogWarning($"Service of type {serviceType.Name} is already registered.");
            }
            else
            {
                serviceDictionary.Add(serviceType, serviceInstance);
            }
        }

        // Get a service instance
        public T GetService<T>()
        {
            Type serviceType = typeof(T);
            if (serviceDictionary.ContainsKey(serviceType))
            {
                return (T)serviceDictionary[serviceType];
            }

            Debug.LogWarning($"Service of type {serviceType.Name} is not registered.");
            return default(T);
        }
    }
}