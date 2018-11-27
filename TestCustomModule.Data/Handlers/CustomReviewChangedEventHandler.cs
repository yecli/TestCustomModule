using Hangfire;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCustomModule.Core.Events;
using TestCustomModule.Core.Model;
using TestCustomModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace TestCustomModule.Web.Handlers
{
	public class CustomerReviewChangedEventHandler : IEventHandler<CustomerReviewChangedEvent>
	{
		private readonly IProductRatingService _productRatingService;

		public CustomerReviewChangedEventHandler(IProductRatingService productRatingService)
		{
			_productRatingService = productRatingService;
		}

		public Task Handle(CustomerReviewChangedEvent message)
		{
			var changedProductIds = GetChangedProductIds(message);
			if (changedProductIds.Length > 0)
			{
				BackgroundJob.Enqueue<IProductRatingService>(x => x.CalculateProductRatings(changedProductIds));
			}
			return Task.CompletedTask;
		}

		private string[] GetChangedProductIds(CustomerReviewChangedEvent message)
		{
			var changedProductIds = new HashSet<string>();
			foreach (var entry in message.ChangedEntries)
			{
				if (entry.EntryState == EntryState.Added)
				{
					AddProductId(changedProductIds, entry.NewEntry);
				}
				else if (entry.EntryState == EntryState.Modified)
				{
					AddProductId(changedProductIds, entry.NewEntry);
					AddProductId(changedProductIds, entry.OldEntry);
				}
				else if (entry.EntryState == EntryState.Deleted)
				{
					AddProductId(changedProductIds, entry.OldEntry);
				}
			}

			return changedProductIds.ToArray();
		}

		private void AddProductId(HashSet<string> ids, CustomerReview review)
		{
			if (review != null && !ids.Contains(review.ProductId))
			{
				ids.Add(review.ProductId);
			}
		}
	}
}
