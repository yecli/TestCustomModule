using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TestCustomModule.Core.Model;
using TestCustomModule.Core.Services;
using TestCustomModule.Data.Services;
using Xunit;

namespace TestCustomModule.Test
{
	public class ProductRatingCalculatorTests
	{
		private const string NonExistentProductId = "nonExistentProductId";
		private const string Product1Id = "1";
		private const string Product2Id = "2";
		private const string Product3Id = "3";

		[Fact]
		public void NoRankingsOnReviews()
		{
			var allReviews = new List<CustomerReview>() {
				new CustomerReview() { ProductId = Product1Id},
				new CustomerReview() { ProductId = Product1Id},
				new CustomerReview() { ProductId = Product1Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product3Id},
			};
			var calculator = GetCalculator(allReviews);
			Assert.Equal(0m, calculator.GetProductRating(Product1Id));
			Assert.Equal(0m, calculator.GetProductRating(Product2Id));
			Assert.Equal(0m, calculator.GetProductRating(Product3Id));
		}

		[Fact]
		public void RankingMinCount()
		{
			var allReviews = new List<CustomerReview>() {
				new CustomerReview() { ProductId = Product1Id, Rating = 1},
				new CustomerReview() { ProductId = Product2Id, Rating = 1},
				new CustomerReview() { ProductId = Product1Id, Rating = 1},
			};
			var calculator = GetCalculator(allReviews.Take(2).ToList());
			Assert.Equal(0m, calculator.GetProductRating(Product1Id));
			Assert.Equal(0m, calculator.GetProductRating(Product2Id));
			Assert.Equal(0m, calculator.GetProductRating(Product3Id));

			calculator = GetCalculator(allReviews);
			Assert.Equal(1m, calculator.GetProductRating(Product1Id));
			Assert.Equal(0m, calculator.GetProductRating(Product2Id));
			Assert.Equal(0m, calculator.GetProductRating(Product3Id));
		}

		[Fact]
		public void BadArguments()
		{
			var allReviews = new List<CustomerReview>() { };
			var calculator = GetCalculator(allReviews);
			Assert.Throws<ArgumentNullException>(() => calculator.GetProductRating(null));
			Assert.Equal(0m, calculator.GetProductRating(NonExistentProductId));
		}

		[Fact]
		public void NotExistentProductId()
		{
			var allReviews = new List<CustomerReview>() {
				new CustomerReview() { ProductId = Product1Id},
				new CustomerReview() { ProductId = Product1Id},
				new CustomerReview() { ProductId = Product1Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product2Id},
				new CustomerReview() { ProductId = Product3Id},
			};
			var calculator = GetCalculator(allReviews);
			Assert.Equal(0m, calculator.GetProductRating(NonExistentProductId));
		}

		[Fact]
		public void ComplexScenario()
		{
			var allReviews = new List<CustomerReview>() {
				new CustomerReview() { ProductId = Product1Id, Rating = 1},
				new CustomerReview() { ProductId = Product1Id, Rating = 2},
				new CustomerReview() { ProductId = Product1Id, Rating = 3},
				new CustomerReview() { ProductId = Product1Id, Rating = 4},
				new CustomerReview() { ProductId = Product1Id, Rating = 5},
				new CustomerReview() { ProductId = Product2Id, Rating = 2},
				new CustomerReview() { ProductId = Product2Id, Rating = 1},
				new CustomerReview() { ProductId = Product3Id, Rating = 2},
				new CustomerReview() { ProductId = Product3Id, Rating = 5},
				new CustomerReview() { ProductId = Product3Id, Rating = 5},
			};
			var calculator = GetCalculator(allReviews);
			Assert.Equal(3m, calculator.GetProductRating(Product1Id));
			Assert.Equal(2.25m, calculator.GetProductRating(Product2Id));
			Assert.Equal(3.6m, calculator.GetProductRating(Product3Id));
		}

		[Fact]
		public void HugeReviewCountTest()
		{
			var allReviews = new List<CustomerReview>()
			{
				new CustomerReview() { ProductId = Product2Id, Rating = 2},
				new CustomerReview() { ProductId = Product2Id, Rating = 2},
			};
			for (int i = 0; i < 150; i++)
			{
				allReviews.Add(new CustomerReview() { ProductId = Product1Id, Rating = i % 5 + 1 });
			}
			var calculator = GetCalculator(allReviews);
			Assert.Equal(Math.Round((150m * 3 + 454m / 152 * 2) / (150 + 2), 10, MidpointRounding.AwayFromZero), Math.Round(calculator.GetProductRating(Product1Id), 10, MidpointRounding.AwayFromZero));
			Assert.Equal(Math.Round((2m * 2 + 454m / 152 * 2) / (2 + 2), 10, MidpointRounding.AwayFromZero), Math.Round(calculator.GetProductRating(Product2Id), 10, MidpointRounding.AwayFromZero));
		}


		private IProductRatingCalculator GetCalculator(ICollection<CustomerReview> reviews)
		{
			var searchServiceMock = CreateSearchService(reviews);
			var calculator = CreateCalculator(searchServiceMock);
			return calculator;
		}

		private static ICustomerReviewSearchService CreateSearchService(ICollection<CustomerReview> allReviews)
		{
			return CreateCustomerServiceMock(allReviews);
		}

		private static ICustomerReviewSearchService CreateCustomerServiceMock(ICollection<CustomerReview> allReviews)
		{
			var searchServiceMock = new Mock<ICustomerReviewSearchService>();
			searchServiceMock.Setup(x => x.SearchCustomerReviews(It.IsAny<CustomerReviewSearchCriteria>()))
				.Returns((CustomerReviewSearchCriteria criteria) =>
				{
					var query = allReviews.AsQueryable();
					if (criteria.ProductIds != null)
					{
						query = query.Where(x => criteria.ProductIds.Contains(x.ProductId));
					}
					if (criteria.HasRating.HasValue && criteria.HasRating.Value)
					{
						query = query.Where(x => x.Rating > 0);
					}
					var results = query.Skip(criteria.Skip)
									 .Take(criteria.Take)
									 .ToList();
					return new VirtoCommerce.Domain.Commerce.Model.Search.GenericSearchResult<CustomerReview>()
					{
						Results = results,
						TotalCount = results.Count,
					};
				});
			return searchServiceMock.Object;
		}

		private IProductRatingCalculator CreateCalculator(ICustomerReviewSearchService searchService)
		{
			return new ProductRatingCalculator(searchService);
		}
	}
}
