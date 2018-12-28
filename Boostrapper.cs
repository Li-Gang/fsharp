using Prism.Unity;
using RedemptionReportGenerator.DAL;
using RedemptionReportGenerator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using System.Windows;
using RedemptionReportGenerator.Presentation.ViewModels;
using RedemptionReportGenerator.Presentation.Views;
using RedemptionReportGenerator.Presentation.Services;
using RedemptionReportGenerator.Domain.RedemptionTrade;
using RedemptionReportGenerator.Domain.Branch;
using RedemptionReportGenerator.Domain.Customer;
using RedemptionReportGenerator.Domain.Localization;
using Prism.Events;
using AutoMapper;
using MvvmDialogs;
using RedemptionReportGenerator.Domain.RedemptionSchedule;
using RedemptionReportGenerator.Domain.Exporter;
using RedemptionReportGenerator.Domain.Interop;
using RedemptionReportGenerator.Domain.Utils;

namespace RedemptionReportGenerator
{
    class Bootstrapper : UnityBootstrapper, IDisposable
    {
        private App MyApp { get; set; }

        public Bootstrapper(App app)
        {
            MyApp = app;
        }

        private void RegisterDAL()
        {
            Container.RegisterType<IUnitOfWork, UnitOfWork>(new PerResolveLifetimeManager());

            Container.RegisterType<UnitOfWork.GetBprTradeDBConnectionString>(
                new InjectionFactory(c =>
                    new UnitOfWork.GetBprTradeDBConnectionString(() => MyApp.GetConnectionString("BprTradeDBDbContext"))));
            Container.RegisterType<UnitOfWork.GetTradeDBConnectionString>(
                new InjectionFactory(c =>
                new UnitOfWork.GetTradeDBConnectionString(() => MyApp.GetConnectionString("TradeDBDbContext"))));
        }

        private void RegisterPresentationLayer()
        {
            Container.RegisterType<IMainWindowViewModel, MainWindowViewModel>();
            Container.RegisterType<ITradeListPageViewModel, TradeListPageViewModel>();
            Container.RegisterType<IRedemptionScheduleListWindowViewModel, RedemptionScheduleListWindowViewModel>();

            Container.RegisterType<FactoryMethods.CreateTradeListPageVM>(
                new InjectionFactory(c =>
                    new FactoryMethods.CreateTradeListPageVM(() => c.Resolve<ITradeListPageViewModel>())));
            Container.RegisterType<FactoryMethods.CreateRedemptionScheduleListWindowVM>(
                new InjectionFactory(c =>
                    new FactoryMethods.CreateRedemptionScheduleListWindowVM(() => c.Resolve<IRedemptionScheduleListWindowViewModel>())));

            Container.RegisterType<ILocalizationService, ApplicationLocalizationService>();
            Container.RegisterType<IRedemptionTradeVMService, RedemptionTradeVMService>();
            Container.RegisterInstance<IVersionService>(MyApp);
            Container.RegisterInstance<IThemeService>(MyApp);

            Container.RegisterInstance<IDialogService>(new DialogService());
        }

        private void RegisterDomainLayer()
        {
            Container.RegisterType<IRedemptionTradeService, RedemptionTradeService>();
            Container.RegisterType<IRedemptionScheduleService, RedemptionScheduleService>();
            Container.RegisterType<ICustomerService, CustomerService>();
            Container.RegisterType<IBranchService, BranchService>();

            Container.RegisterInstance<IExcelService>(new ExcelService());

            Container.RegisterType<IKIDateStringConverter, KIDateStringConverter>();
            Container.RegisterType<IStringToDecimalConverter, StringToDecimalConverter>();
        }

        private void RegisterApplicationWiseServices()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(typeof(App));
            });

            var mapper = config.CreateMapper();
            Container.RegisterInstance(mapper);

            Container.RegisterType<IUserSettingsService, ApplicationUserSettingsService>();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            RegisterApplicationWiseServices();

            RegisterPresentationLayer();
            RegisterDAL();
            RegisterDomainLayer();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        public void Dispose()
        {
            Container.Resolve<IExcelService>().Dispose();
        }
    }
}
