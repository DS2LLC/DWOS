using GalaSoft.MvvmLight.Ioc;
using System;

namespace DWOS.Utilities
{
    /// <summary>
    /// Inversion of control implementation.
    /// </summary>
    public static class ServiceContainer
    {
        #region Methods

        /// <summary>
        /// Registers an instance with the default container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public static void Register<T>(T service) where T : class =>
            SimpleIoc.Default.Register(() => service);

        /// <summary>
        /// Registers a concrete type with the default container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>() where T : class =>
            SimpleIoc.Default.Register<T>();

        /// <summary>
        /// Registers a function with the default container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        public static void Register<T>(Func<T> function) where T : class =>
            SimpleIoc.Default.Register(function);

        /// <summary>
        /// Resolves an instance using the default container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The registered instance if found</returns>
        public static T Resolve<T>() =>
            SimpleIoc.Default.GetInstance<T>();

        #endregion
    }
}