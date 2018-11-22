using System;
using System.Linq;
using TestCustomModule.Core.Services;

namespace TestCustomModule.ProductRatingCalculator
{
	public class ProductRaitingCalculator : IProductRaitingCalculator
	{
		public static readonly int MinReviewCountLimit = 2;

		private ICustomerReviewSearchService reviewSearchService;

		public ProductRaitingCalculator(ICustomerReviewSearchService reviewSearchService)
		{
			this.reviewSearchService = reviewSearchService;
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

			//Read all items at once in this demo project
			var searchResult = reviewSearchService.SearchCustomerReviews(new Core.Model.CustomerReviewSearchCriteria()
			{
				ProductIds = new string[] { productId },
				HasRating = true,
			});
			if (searchResult != null && searchResult.TotalCount > 0)
			{
				var specificProductReviews = searchResult.Results.ToList();
				var specificProductReviewsCount = specificProductReviews.Count;
				if (specificProductReviewsCount >= MinReviewCountLimit)
				{
					//rating = (V*R + M*C)/(V+M)
					//Where
					//V – This product review count
					//M – Review count limit for product to have rating
					//R – Average rating for this product
					//С – Average rating for all products

					var specificAverage = specificProductReviews.Average(x => x.Rating);

					//Getting all in one butch for now
					var allReviewAverageRating = reviewSearchService.SearchCustomerReviews(new Core.Model.CustomerReviewSearchCriteria() { }).Results.Average(x => x.Rating);

					result = (decimal)(specificProductReviewsCount * specificAverage + MinReviewCountLimit * allReviewAverageRating) / (specificProductReviewsCount + MinReviewCountLimit);

				}
			}
			return result;
		}
	}
}
