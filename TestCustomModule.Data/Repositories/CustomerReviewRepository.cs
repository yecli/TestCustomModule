using System.Data.Entity;
using System.Linq;
using TestCustomModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace TestCustomModule.Data.Repositories
{
	public class CustomerReviewRepository : EFRepositoryBase, ICustomerReviewRepository, IProductRatingRepository
	{
		public CustomerReviewRepository()
		{
		}

		public CustomerReviewRepository(string nameOrConnectionString, params IInterceptor[] interceptors)
			: base(nameOrConnectionString, null, interceptors)
		{
			Configuration.LazyLoadingEnabled = false;
		}

		#region ICustomerReviewRepository

		public IQueryable<CustomerReviewEntity> CustomerReviews => GetAsQueryable<CustomerReviewEntity>();

		public CustomerReviewEntity[] GetByIds(string[] ids)
		{
			return CustomerReviews.Where(x => ids.Contains(x.Id)).ToArray();
		}

		public void DeleteCustomerReviews(string[] ids)
		{
			var items = GetByIds(ids);
			foreach (var item in items)
			{
				Remove(item);
			}
		}
		#endregion

		#region IProductRatingRepository

		public IQueryable<ProductRatingEntity> ProductRatings => GetAsQueryable<ProductRatingEntity>();

		public ProductRatingEntity[] GetByProductIds(string[] productIds)
		{
			return ProductRatings.Where(x => productIds.Contains(x.ProductId)).ToArray();
		}

		#endregion

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CustomerReviewEntity>().ToTable("CustomerReview").HasKey(x => x.Id).Property(x => x.Id);

			modelBuilder.Entity<ProductRatingEntity>().ToTable("ProductRating").HasKey(x => x.Id).Property(x => x.Id);

			base.OnModelCreating(modelBuilder);
		}
	}
}
