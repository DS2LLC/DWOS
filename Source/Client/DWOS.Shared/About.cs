using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace DWOS.Shared
{
    /// <summary>
    /// Defines utility methods for retrieving Application/Assembly information.
    /// </summary>
    public static class About
    {
        #region Fields

        /// <summary>
        /// List of assemblies registered with the about class.
        /// </summary>
        private static List<Assembly> _registeredAssemblies = new List<Assembly>();

        /// <summary>
        /// List of invlaid beggining names of assemblies.
        /// </summary>
        public static string[] InvalidAssemblyBeginningNames = new[] {"GSD", "System", "Infragistics", "ESRI"};

        #endregion

        #region Properties

        /// <summary>
        /// Get/Set the current assemblies name.
        /// </summary>
        public static Assembly CurrentAssembly
        {
            get
            {
                //if only one registered assembly
                if(_registeredAssemblies.Count == 1)
                    return _registeredAssemblies[0];

                //more than one assembly registered determine which one is calling
                if(_registeredAssemblies.Count > 1)
                    return FindRegisteredCallingAssembly();
                else
                {
                    //if no registered assemblies then find calling assembly and add register it
                    Assembly callingAssembly = FindValidCallingAssembly();
                    RegisterAssembly(callingAssembly);
                    return callingAssembly;
                }
            }
            set
            {
                //Kept for backwards compatibality.
                RegisterAssembly(value);
            }
        }

        /// <summary>
        /// Gets the application name for the current assembly.
        /// </summary>
        public static string ApplicationName
        {
            get { return GetApplicationName(CurrentAssembly); }
        }

        /// <summary>
        /// Gets the copyright information for the current assembly.
        /// </summary>
        public static string ApplicationCopyright
        {
            get
            {
                var atCopyright =
                    (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(CurrentAssembly, typeof(AssemblyCopyrightAttribute));
                return atCopyright.Copyright;
            }
        }

        /// <summary>
        /// Gets the company name for the current assembly.
        /// </summary>
        public static string ApplicationCompany
        {
            get
            {
                var atCompany =
                    (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(CurrentAssembly, typeof(AssemblyCompanyAttribute));
                return atCompany.Company;
            }
        }

        /// <summary>
        /// Gets the title for the current assembly.
        /// </summary>
        public static string ApplicationTitle
        {
            get { return GetApplicationTitle(CurrentAssembly); }
        }

        /// <summary>
        /// Gets the description for the current assembly.
        /// </summary>
        public static string ApplicationDesciption
        {
            get { return GetApplicationDescription(CurrentAssembly); }
        }

        /// <summary>
        /// Get the configuration for the current assembly.
        /// </summary>
        public static string ApplicationConfiquration
        {
            get { return GetApplicationConfiquration(CurrentAssembly); }
        }

        /// <summary>
        /// Get the trademark for the current assembly.
        /// </summary>
        public static string ApplicationTrademark
        {
            get
            {
                var atTrade =
                    (AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(CurrentAssembly, typeof(AssemblyTrademarkAttribute));
                return atTrade.Trademark;
            }
        }

        /// <summary>
        /// Get the version of the current assembly.
        /// </summary>
        public static string ApplicationVersion
        {
            get { return GetApplicationVersion(CurrentAssembly); }
        }

        /// <summary>
        /// Get the major-minor version of the current assembly.
        /// </summary>
        public static string ApplicationVersionMajorMinor
        {
            get { return GetApplicationVersionMajorMinor(CurrentAssembly); }
        }

        /// <summary>
        /// Gets the Raygun API key for the current assembly.
        /// </summary>
        public static string RaygunApiKey
        {
            get { return GetRaygunApiKey(CurrentAssembly); }
        }

        /// <summary>
        /// Gets the release date for teh current assembly.
        /// </summary>
        /// <value>
        /// The assembly's release date if found and valid; otherwise, null.
        /// </value>
        public static DateTime? ApplicationReleaseDate
        {
            get
            {
                return GetReleaseDate(CurrentAssembly);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determine if the assembly name is valid.
        /// </summary>
        /// <remarks>
        /// Compares it to the InvalidAssemblyBeginningNames strings
        /// </remarks>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private static bool ValidAssemblyName(string assemblyName)
        {
            foreach(string invalidName in InvalidAssemblyBeginningNames)
            {
                if(assemblyName.StartsWith(invalidName))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Find the valid calling assembly.
        /// </summary>
        /// <returns></returns>
        private static Assembly FindValidCallingAssembly()
        {
            var stackTrace = new StackTrace(1, false);

            for(int i = stackTrace.FrameCount - 1; i >= 0; i--)
            {
                StackFrame sf = stackTrace.GetFrame(i);
                MethodBase mb = sf.GetMethod();

                //ensure not the GSD Framework
                if(mb != null && ValidAssemblyName(mb.DeclaringType.Namespace))
                    return mb.DeclaringType.Assembly;
            }

            Debug.Assert(false, "Unable to find a valid calling assembly in the call stack.",
                         "Ensure that your assembly has been registered with the About class at startup.");
            return Assembly.GetCallingAssembly();
        }

        /// <summary>
        /// Find the valid calling registered assembly.
        /// </summary>
        /// <returns></returns>
        private static Assembly FindRegisteredCallingAssembly()
        {
            var stackTrace = new StackTrace(1, false);

            for(int i = stackTrace.FrameCount - 1; i >= 0; i--)
            {
                StackFrame sf = stackTrace.GetFrame(i);
                MethodBase mb = sf.GetMethod();

                //if the assembly is registered than return it
                if(_registeredAssemblies.Contains(mb.DeclaringType.Assembly))
                    return mb.DeclaringType.Assembly;
            }

            Debug.Assert(false, "Unable to find the registered assembly in the call stack.",
                         "Ensure that your assembly has been registered with the About class at startup.");
            return Assembly.GetCallingAssembly();
        }

        /// <summary>
        /// Register the assembly so that the About class can use it, instead of having to always search the call stack.
        /// </summary>
        /// <param name="assembly"></param>
        public static void RegisterAssembly(Assembly assembly)
        {
            if(_registeredAssemblies == null)
                _registeredAssemblies = new List<Assembly>();

            if(assembly != null && !_registeredAssemblies.Contains(assembly))
                _registeredAssemblies.Add(assembly);
        }

        /// <summary>
        /// Get the Applications Name
        /// </summary>
        public static string GetApplicationName(Assembly assembly)
        {
            var atProduct =
                (AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute));
            return atProduct.Product;
        }

        /// <summary>
        /// Get the Application Description
        /// </summary>
        public static string GetApplicationDescription(Assembly assembly)
        {
            var atDesc = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute));
            return atDesc.Description;
        }

        /// <summary>
        /// Get the Application Version
        /// </summary>
        public static string GetApplicationVersion(Assembly assembly)
        {
            return assembly.GetName().Version.ToString();
        }

        /// <summary>
        /// Get the Application Configuration
        /// </summary>
        public static string GetApplicationConfiquration(Assembly assembly)
        {
            var atConfig =
                (AssemblyConfigurationAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyConfigurationAttribute));
            return atConfig.Configuration;
        }

        /// <summary>
        /// Get the Application Title
        /// </summary>
        public static string GetApplicationTitle(Assembly assembly)
        {
            var atTitle =
                (AssemblyTitleAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute));
            return atTitle.Title;
        }

        /// <summary>
        /// Get the Application Version
        /// </summary>
        public static string GetApplicationVersionMajorMinor(Assembly assembly)
        {
            var ver = new Version(GetApplicationVersion(assembly));
            return ver.Major + "." + ver.Minor;
        }

        /// <summary>
        /// Get the Raygun API key for an assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetRaygunApiKey(Assembly assembly)
        {
            var attribute = Attribute.GetCustomAttribute(assembly, typeof(RaygunApiKeyAttribute)) as RaygunApiKeyAttribute;
            return attribute == null ? null : attribute.ApiKey;
        }

        /// <summary>
        /// Get the release date for an assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static DateTime? GetReleaseDate(Assembly assembly)
        {
            var attribute = Attribute.GetCustomAttribute(assembly,
                typeof(AssemblyReleaseDateAttribute)) as AssemblyReleaseDateAttribute;

            return attribute?.Date;
        }

        #endregion
    }
}