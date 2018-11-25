using Microsoft.Practices.Unity;
using TestCustomModule.Core.Events;
using TestCustomModule.Core.Services;
using TestCustomModule.Data.Repositories;
using TestCustomModule.Data.Services;
using TestCustomModule.ProductRatingCalculator;
using TestCustomModule.Web.Handlers;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace TestCustomModule.Web
{
	public class Module : ModuleBase
	{
		private readonly string _connectionString = ConfigurationHelper.GetConnectionStringValue("VirtoCommerce.CustomerReviews") ?? ConfigurationHelper.GetConnectionStringValue("VirtoCommerce");
		private readonly IUnityContainer _container;

		public Module(IUnityContainer container)
		{
			_container = container;
		}

		public override void SetupDatabase()
		{
			using (var db = new CustomerReviewRepository(_connectionString, _container.Resolve<AuditableInterceptor>()))
			{
				var initializer = new SetupDatabaseInitializer<CustomerReviewRepository, Data.Migrations.Configuration>();
				initializer.InitializeDatabase(db);
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			// This method is called for each installed module on the first stage of initialization.

			// Register implementations:
			var injectionFactory = new InjectionFactory(c => new CustomerReviewRepository(_connectionString, new EntityPrimaryKeyGeneratorInterceptor(), _container.Resolve<AuditableInterceptor>()));

			_container.RegisterType<ICustomerReviewRepository>(injectionFactory);
			_container.RegisterType<ICustomerReviewService, CustomerReviewService>();
			_container.RegisterType<ICustomerReviewSearchService, CustomerReviewSearchService>();

			_container.RegisterType<IProductRatingRepository>(injectionFactory);
			_container.RegisterType<IProductRatingService, ProductRatingService>();
			_container.RegisterType<IProductRaitingCalculator, ProductRaitingCalculator>();

			var eventHandlerRegistrar = _container.Resolve<IHandlerRegistrar>();
			eventHandlerRegistrar.RegisterHandler<CustomerReviewChangedEvent>(async (message, token) => await _container.Resolve<CustomerReviewChangedEventHandler>().Handle(message));

			// Try to avoid calling _container.Resolve<>();
		}

		public override void PostInitialize()
		{
			base.PostInitialize();

			// This method is called for each installed module on the second stage of initialization.

			// Register implementations 
			// _container.RegisterType<IMyService, MyService>();

			// Resolve registered implementations:
			var settingManager = _container.Resolve<ISettingsManager>();
			var value = settingManager.GetValue("Pricing.ExportImport.Description", string.Empty);
		}
	}
}
