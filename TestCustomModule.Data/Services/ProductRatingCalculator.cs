using System;
using System.Collections.Generic;
using System.Linq;
using TestCustomModule.Core.Model;
using TestCustomModule.Core.Services;
using TestCustomModule.Data.Extensions;

namespace TestCustomModule.Data.Services
{
	public class ProductRatingCalculator : IProductRatingCalculator
	{
		public static readonly int MinReviewCountLimit = 2;

		private readonly ICustomerReviewSearchService _reviewSearchService;

		public ProductRatingCalculator(ICustomerReviewSearchService reviewSearchService)
		{
			this._reviewSearchService = reviewSearchService;
		}

		/// <summary>
		/// Gets the aggregated product rating based on formula from https://habr.com/post/172065/
		/// </summary>
		/// <param name="productId"></param>
		/// <returns></returns>
		public decimal GetProductRating(string productId)
		{
			if (string.IsNullOrEmpty(productId))
			{
				throw new ArgumentNullException(nameof(productId));
			}

			var result = 0m;

			var allReviews = GetAllProductReviews().ToArray();
			var specificProductRevieswWithRating = allReviews.Where(x => x.ProductId.Equals(productId, StringComparison.OrdinalIgnoreCase) && x.Rating > 0).ToArray();
			var specificProductReviewsCount = specificProductRevieswWithRating.Length;
			if (specificProductReviewsCount >= MinReviewCountLimit)
			{
				//rating = (V*R + M*C)/(V+M)
				//Where
				//V – This product review count
				//M – Review count limit for product to have rating
				//R – Average rating for this product
				//С – Average rating for all products

				var specificAverage = specificProductRevieswWithRating.Average(x => x.Rating);

				var allReviewAverageRating = allReviews.Average(x => x.Rating);

				result = (decimal)(specificProductReviewsCount * specificAverage + MinReviewCountLimit * allReviewAverageRating) / (specificProductReviewsCount + MinReviewCountLimit);

			}
			return result;
		}

		private IEnumerable<CustomerReview> GetAllProductReviews()
		{
			var searchCriteria = new CustomerReviewSearchCriteria();

			var batchesEnumerable = searchCriteria.ReadAllEntitiesByBatches(x => _reviewSearchService.SearchCustomerReviews(x).Results);
			return batchesEnumerable.SelectMany(x => x);
		}
	}
}
