using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using DWOS.Shared;
using NLog;

namespace DWOS.DataImporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            System.Windows.Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            About.RegisterAssembly(Assembly.GetExecutingAssembly());

            LogManager.GetCurrentClassLogger().Info("Application starting...");
        }


        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            LogManager.GetCurrentClassLogger().Info("Error resolving assembly: " + args.Name);

            Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly ass in asses)
            {
                if (ass.GetName().Name == args.Name)
                {
                    LogManager.GetCurrentClassLogger().Info("Resolved assembly as: " + ass.FullName);
                    return ass;
                }
            }

            return null;
        }

        private static void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogManager.GetCurrentClassLogger().Error(e.Exception, "An application error has occurred.");
        }

    }
}
