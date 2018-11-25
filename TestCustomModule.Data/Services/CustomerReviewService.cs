using System;
using System.Collections.Generic;
using System.Linq;
using TestCustomModule.Core.Events;
using TestCustomModule.Core.Model;
using TestCustomModule.Core.Services;
using TestCustomModule.Data.Model;
using TestCustomModule.Data.Repositories;
using VirtoCommerce.Domain.Common.Events;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace TestCustomModule.Data.Services
{
	public class CustomerReviewService : ServiceBase, ICustomerReviewService
	{
		private readonly Func<ICustomerReviewRepository> _repositoryFactory;
		private readonly IEventPublisher _eventPublisher;

		public CustomerReviewService(Func<ICustomerReviewRepository> repositoryFactory, IEventPublisher eventPublisher)
		{
			_repositoryFactory = repositoryFactory;
			_eventPublisher = eventPublisher;
		}

		public CustomerReview[] GetByIds(string[] ids)
		{
			using (var repository = _repositoryFactory())
			{
				return repository.GetByIds(ids).Select(x => x.ToModel(AbstractTypeFactory<CustomerReview>.TryCreateInstance())).ToArray();
			}
		}

		public void SaveCustomerReviews(CustomerReview[] items)
		{
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			var pkMap = new PrimaryKeyResolvingMap();
			var changedEntries = new List<GenericChangedEntry<CustomerReview>>();
			using (var repository = _repositoryFactory())
			{
				using (var changeTracker = GetChangeTracker(repository))
				{
					var alreadyExistEntities = repository.GetByIds(items.Where(m => !m.IsTransient()).Select(x => x.Id).ToArray());
					foreach (var derivativeContract in items)
					{
						var sourceEntity = AbstractTypeFactory<CustomerReviewEntity>.TryCreateInstance().FromModel(derivativeContract, pkMap);
						var targetEntity = alreadyExistEntities.FirstOrDefault(x => x.Id == sourceEntity.Id);
						if (targetEntity != null)
						{
							changeTracker.Attach(targetEntity);
							sourceEntity.Patch(targetEntity);
						}
						else
						{
							repository.Add(sourceEntity);
						}
						changedEntries.Add(new GenericChangedEntry<CustomerReview>(derivativeContract, (targetEntity != null) ? EntryState.Modified : EntryState.Added));
					}

					CommitChanges(repository);
					pkMap.ResolvePrimaryKeys();
				}
				_eventPublisher.Publish(new CustomerReviewChangedEvent(changedEntries));
			}
		}

		public void DeleteCustomerReviews(string[] ids)
		{
			using (var repository = _repositoryFactory())
			{
				repository.DeleteCustomerReviews(ids);
				CommitChanges(repository);
				_eventPublisher.Publish(new CustomerReviewChangedEvent(repository.GetByIds(ids).Select(x =>
					new GenericChangedEntry<CustomerReview>(x.ToModel(AbstractTypeFactory<CustomerReview>.TryCreateInstance()), EntryState.Deleted))));
			}
		}
	}
}
