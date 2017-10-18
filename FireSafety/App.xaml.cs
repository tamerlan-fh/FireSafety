using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
            //if (args.Name.Contains("Spire.License"))
            //    return Assembly.Load(ЗданиеWpf.Properties.Resources.Spire_License);
            //if (args.Name.Contains("Spire.Pdf"))
            //    return Assembly.Load(ЗданиеWpf.Properties.Resources.Spire_Pdf);
            if (args.Name.Contains("DocX"))
                return Assembly.Load(FireSafety.Properties.Resources.DocX);

            return null;
        }
    }
}
