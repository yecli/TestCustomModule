using TestCustomModule.Core.Model;

namespace TestCustomModule.Core.Services
{
	public interface IProductRatingService
	{
		ProductRating[] GetProductRatings(string[] productIds);
		void CalculateProductRatings(string[] productIds);
		void SaveProductRatings(ProductRating[] ratings);
	}
}
