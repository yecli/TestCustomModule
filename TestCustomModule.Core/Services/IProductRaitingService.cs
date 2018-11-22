namespace TestCustomModule.Core.Services
{
	public interface IProductRaitingService
	{
		decimal GetProductRating(string productId);
		void UpdateProductRating(string productId);
	}
}
