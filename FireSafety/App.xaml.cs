using System;
using System.Reflection;
using System.Windows;

namespace FireSafety
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("DocX"))
                return Assembly.Load(FireSafety.Properties.Resources.DocX);

            return null;
        }
    }
}
