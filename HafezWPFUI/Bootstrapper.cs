using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HafezLibrary.Models;
using HafezWPFUI.Models;
using HafezWPFUI.ViewModels.Windows;

namespace HafezWPFUI
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new();

        public Bootstrapper()
        {
            Initialize();
        }

        public static IMapper ConfigureAutoMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PersonalDuaListModel, PersonalDuaListDisplayModel>();
                cfg.CreateMap<PersonalDuaListDisplayModel, PersonalDuaListModel>();
            });

            IMapper output = config.CreateMapper();

            return output;
        }

        protected override void Configure()
        {
            _container.Instance(ConfigureAutoMapper());

            _container.Instance(_container);
            // .PerRequest<IProductEndpoint, ProductEndpoint>()
            // .PerRequest<IUserEndpoint, UserEndpoint>()
            // .PerRequest<ISaleEndpoint, SaleEndpoint>();

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();
            // .Singleton<MainWindowView>()
            // .Singleton<OutputWindowView>()
            // .Singleton<ListenerWindowView>();

            GetType().Assembly.GetTypes()
                     .Where(type => type.IsClass)
                     .Where(type => type.Name.EndsWith("ViewModel"))
                     .ToList()
                     .ForEach(viewModelType => _container.RegisterPerRequest(
                                                                             viewModelType, viewModelType.ToString(),
                                                                             viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}