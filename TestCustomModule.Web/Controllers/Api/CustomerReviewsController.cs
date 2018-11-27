using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using TestCustomModule.Core.Model;
using TestCustomModule.Core.Services;
using TestCustomModule.Web.Model;
using TestCustomModule.Web.Security;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Web.Security;

namespace TestCustomModule.Web.Controllers.Api
{
	[RoutePrefix("api/customerReviews")]
	public class CustomerReviewsController : ApiController
	{
		private readonly ICustomerReviewSearchService _customerReviewSearchService;
		private readonly ICustomerReviewService _customerReviewService;
		private readonly IProductRatingService _productRatingService;

		public CustomerReviewsController()
		{
		}

		public CustomerReviewsController(ICustomerReviewSearchService customerReviewSearchService, ICustomerReviewService customerReviewService, IProductRatingService productRatingService)
		{
			_customerReviewSearchService = customerReviewSearchService;
			_customerReviewService = customerReviewService;
			_productRatingService = productRatingService;

		}

		/// <summary>
		/// Return product Customer review search results
		/// </summary>
		[HttpPost]
		[Route("search")]
		[ResponseType(typeof(GenericSearchResult<CustomerReview>))]
		[CheckPermission(Permission = PredefinedPermissions.CustomerReviewRead)]
		public IHttpActionResult SearchCustomerReviews(CustomerReviewSearchCriteria criteria)
		{
			var result = _customerReviewSearchService.SearchCustomerReviews(criteria);
			return Ok(result);
		}

		/// <summary>
		///  Create new or update existing customer review
		/// </summary>
		/// <param name="customerReviews">Customer reviews</param>
		/// <returns></returns>
		[HttpPut]
		[Route("")]
		[ResponseType(typeof(void))]
		[CheckPermission(Permission = PredefinedPermissions.CustomerReviewUpdate)]
		public IHttpActionResult Update(CustomerReview[] customerReviews)
		{
			_customerReviewService.SaveCustomerReviews(customerReviews);
			return StatusCode(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Delete Customer Reviews by IDs
		/// </summary>
		/// <param name="ids">IDs</param>
		/// <returns></returns>
		[HttpDelete]
		[Route("")]
		[ResponseType(typeof(void))]
		[CheckPermission(Permission = PredefinedPermissions.CustomerReviewDelete)]
		public IHttpActionResult Delete([FromUri] string[] ids)
		{
			_customerReviewService.DeleteCustomerReviews(ids);
			return StatusCode(HttpStatusCode.NoContent);
		}

		/// <summary>
		/// Return product aggregated rating based on all reviews
		/// </summary>
		[HttpGet]
		[Route("productRating")]
		[ResponseType(typeof(ProductRatingResult))]
		[CheckPermission(Permission = PredefinedPermissions.CustomerReviewRead)]
		public IHttpActionResult GetProductRating(string productId)
		{
			var ratings = _productRatingService.GetProductRatings(new string[] { productId });
			var result = new ProductRatingResult()
			{
				RatingValue = ratings.FirstOrDefault()?.Rating,
			};
			return Ok(result);
		}
	}
}
