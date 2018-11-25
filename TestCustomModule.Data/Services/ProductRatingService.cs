using System;
using System.Linq;
using TestCustomModule.Core.Model;
using TestCustomModule.Core.Services;
using TestCustomModule.Data.Model;
using TestCustomModule.Data.Repositories;
using TestCustomModule.ProductRatingCalculator;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace TestCustomModule.Data.Services
{
	public class ProductRatingService : ServiceBase, IProductRatingService
	{
		private readonly Func<IProductRatingRepository> _repositoryFactory;
		private readonly IProductRaitingCalculator _calculator;

		public ProductRatingService(Func<IProductRatingRepository> repositoryFactory, IProductRaitingCalculator calculator)
		{
			_repositoryFactory = repositoryFactory;
			_calculator = calculator;
		}

		public ProductRating[] GetProductRatings(string[] productIds)
		{
			using (var repository = _repositoryFactory())
			{
				return repository.GetByProductIds(productIds)?.Select(x => x.ToModel(AbstractTypeFactory<ProductRating>.TryCreateInstance())).ToArray();
			}
		}

		public void CalculateProductRatings(string[] productIds)
		{
			if (productIds == null)
				throw new ArgumentNullException(nameof(productIds));

			SaveProductRatings(productIds.Select(x => new ProductRating()
			{
				ProductId = x,
				Rating = _calculator.GetProductRating(x)
			}).ToArray());
		}

		public void SaveProductRatings(ProductRating[] ratings)
		{
			if (ratings == null)
				throw new ArgumentNullException(nameof(ratings));

			var pkMap = new PrimaryKeyResolvingMap();
			using (var repository = _repositoryFactory())
			{
				using (var changeTracker = GetChangeTracker(repository))
				{
					var alreadyExistEntities = repository.GetByProductIds(ratings.Select(x => x.ProductId).ToArray());
					foreach (var derivativeContract in ratings)
					{
						var sourceEntity = AbstractTypeFactory<ProductRatingEntity>.TryCreateInstance().FromModel(derivativeContract, pkMap);
						var targetEntity = alreadyExistEntities.FirstOrDefault(x => x.ProductId == sourceEntity.ProductId);
						if (targetEntity != null)
						{
							changeTracker.Attach(targetEntity);
							sourceEntity.Patch(targetEntity);
						}
						else
						{
							repository.Add(sourceEntity);
						}
					}
				}

				CommitChanges(repository);
				pkMap.ResolvePrimaryKeys();
			}
		}
	}
}
