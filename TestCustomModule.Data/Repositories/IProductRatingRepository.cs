using System.Linq;
using TestCustomModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace TestCustomModule.Data.Repositories
{
	public interface IProductRatingRepository : IRepository
	{
		IQueryable<ProductRatingEntity> ProductRatings { get; }

		ProductRatingEntity[] GetByProductIds(string[] productIds);
	}
}
