using Prism.Mvvm;
using RedemptionReportGenerator.Presentation.Services;
using RedemptionReportGenerator.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Deployment.Application;
using System.Reflection;

namespace RedemptionReportGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IVersionService, IThemeService
    {
        private const string DEV_ENV_SUBFIX = "_dev";
        private const string UAT_ENV_SUBFIX = "_uat";

        public static bool IsUAT
        {
            get
            {
#if UAT
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsDevelopment
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsProduction
        {
            get
            {
                return !IsUAT && !IsDevelopment;
            }
        }

        public string Version
        {
            get
            {
                var versionStr = "";

                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    versionStr = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }
                else
                {
                    versionStr = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }

                if (IsDevelopment)
                {
                    versionStr += DEV_ENV_SUBFIX;
                }
                else if (IsUAT)
                {
                    versionStr += UAT_ENV_SUBFIX;
                }

                return versionStr;
            }
        }

        public int ThemeIndex
        {
            get
            {
                return Settings.Default.ThemeIndex;
            }
            set
            {
                ChangeTheme(value);

                Settings.Default.ThemeIndex = value;
                Settings.Default.Save();
            }
        }

        public int ThemeCount
        {
            get
            {
                var themes = Resources["Themes"] as ArrayList;
                return themes.Count;
            }
        }

        public string GetConnectionString(string name)
        {
            if (IsDevelopment)
            {
                name += DEV_ENV_SUBFIX;
            }
            else if (IsUAT)
            {
                name += UAT_ENV_SUBFIX;
            }

            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        private void ChangeTheme(int themeDictionaryIndex)
        {
            var themes = Resources["Themes"] as ArrayList;
            ChangeTheme(new Uri(themes[themeDictionaryIndex] as string, UriKind.Relative));
        }

        private void ChangeTheme(string themeDictionaryPath)
        {
            ChangeTheme(new Uri(themeDictionaryPath, UriKind.Relative));
        }

        private void ChangeTheme(Uri themeDictionaryUri)
        {
            var themeDictionaries = Resources.MergedDictionaries.OfType<ThemeResourceDictionary>().ToList();
            foreach (var themeDictionary in themeDictionaries)
            {
                Resources.MergedDictionaries.Remove(themeDictionary);
            }

            if (themeDictionaryUri != null)
            {
                var newThemeDictionary = new ThemeResourceDictionary();
                newThemeDictionary.Source = themeDictionaryUri;

                Resources.MergedDictionaries.Insert(4, newThemeDictionary);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var bootstrapper = new Bootstrapper(this);

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewName = viewType.Name;
                var viewAssemblyName = viewType.GetType().Assembly.FullName;
                var viewModelName = string.Format(CultureInfo.InvariantCulture, "RedemptionReportGenerator.Presentation.ViewModels.I{0}ViewModel", viewName);

                return Type.GetType(viewModelName);
            });

            ChangeTheme(ThemeIndex);

            DispatcherUnhandledException += App_DispatcherUnhandledException;
            Exit += (sender, arg) => bootstrapper.Dispose();

            bootstrapper.Run();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            throw e.Exception;
        }

        // Not used directly by any code but necessary to initialize Costura.
        private static void CosturaInit()
        {
            CosturaUtility.Initialize();
        }
    }
}
