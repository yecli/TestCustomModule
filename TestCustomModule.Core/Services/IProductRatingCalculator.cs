namespace TestCustomModule.Core.Services
{
	public interface IProductRatingCalculator
	{
		decimal GetProductRating(string productId);
	}
}
