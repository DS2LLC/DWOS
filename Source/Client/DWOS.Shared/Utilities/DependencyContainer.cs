using System;
using System.Collections.Generic;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Inversion of control implementation.
    /// </summary>
    public class DependencyContainer
    {
        #region Fields

        static object locker = new object();
        static DependencyContainer instance;

        #endregion

        #region Properties

        private static DependencyContainer Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                        instance = new DependencyContainer();
                    return instance;
                }
            }
        }

        private Dictionary<Type, Lazy<object>> Services { get; set; }

        #endregion

        #region Methods

        private DependencyContainer()
        {
            Services = new Dictionary<Type, Lazy<object>>();
        }

        /// <summary>
        /// Registers an instance with the default container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public static void Register<T>(T service)
        {
            Instance.Services[typeof(T)] = new Lazy<object>(() => service);
        }

        /// <summary>
        /// Registers a concrete type with the default container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>()
            where T : new()
        {
            Instance.Services[typeof(T)] = new Lazy<object>(() => new T());
        }

        /// <summary>
        /// Registers a function with the default container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        public static void Register<T>(Func<object> function)
        {
            Instance.Services[typeof(T)] = new Lazy<object>(function);
        }

        /// <summary>
        /// Resolves an instance using the default container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="KeyNotFoundException">
        /// Instance of requested type was not found.
        /// </exception>
        /// <returns>The registered instance if found</returns>
        public static T Resolve<T>()
        {
            Lazy<object> service;
            if (Instance.Services.TryGetValue(typeof(T), out service))
            {
                return (T)service.Value;
            }
            else
            {
                throw new KeyNotFoundException(string.Format("Service not found for type '{0}'", typeof(T)));
            }
        }

        #endregion
    }
}
