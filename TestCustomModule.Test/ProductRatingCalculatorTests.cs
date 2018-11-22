using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TestCustomModule.Core.Model;
using TestCustomModule.Core.Services;
using TestCustomModule.ProductRatingCalculator;
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
			IProductRaitingCalculator calculator = GetCalculator(allReviews);
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
			IProductRaitingCalculator calculator = GetCalculator(allReviews.Take(2).ToList());
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
			IProductRaitingCalculator calculator = GetCalculator(allReviews);
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
			IProductRaitingCalculator calculator = GetCalculator(allReviews);
			Assert.Equal(3m, calculator.GetProductRating(Product1Id));
			Assert.Equal(2.25m, calculator.GetProductRating(Product2Id));
			Assert.Equal(3.6m, calculator.GetProductRating(Product3Id));
		}

		private IProductRaitingCalculator GetCalculator(ICollection<CustomerReview> reviews)
		{
			var searchServiceMock = CreateSearchServiceMock(reviews);
			var calculator = CreateCalculator(searchServiceMock.Object);
			return calculator;
		}

		private static Mock<ICustomerReviewSearchService> CreateSearchServiceMock(ICollection<CustomerReview> allReviews)
		{
			var searchServiceMock = new Mock<ICustomerReviewSearchService>();
			searchServiceMock.Setup(x => x.SearchCustomerReviews(It.IsAny<CustomerReviewSearchCriteria>()))
				.Returns((CustomerReviewSearchCriteria criteria) =>
				{
					var results = criteria != null && criteria.ProductIds != null ? allReviews.Where(x => x.Rating > 0 && criteria.ProductIds.Contains(x.ProductId)).ToList() : allReviews;
					return new VirtoCommerce.Domain.Commerce.Model.Search.GenericSearchResult<CustomerReview>()
					{
						Results = results,
						TotalCount = results.Count,
					};
				});
			return searchServiceMock;
		}

		private IProductRaitingCalculator CreateCalculator(ICustomerReviewSearchService searchService)
		{
			return new ProductRaitingCalculator(searchService);
		}
	}
}
