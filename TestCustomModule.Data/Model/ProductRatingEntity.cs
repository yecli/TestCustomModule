using System;
using System.ComponentModel.DataAnnotations;
using TestCustomModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace TestCustomModule.Data.Model
{
	public class ProductRatingEntity : AuditableEntity
	{
		[Required]
		[StringLength(128)]
		public string ProductId { get; set; }

		[Required]
		public decimal Rating { get; set; }

		public virtual ProductRating ToModel(ProductRating productRating)
		{
			if (productRating == null)
				throw new ArgumentNullException(nameof(productRating));

			productRating.Id = Id;
			productRating.CreatedBy = CreatedBy;
			productRating.CreatedDate = CreatedDate;
			productRating.ModifiedBy = ModifiedBy;
			productRating.ModifiedDate = ModifiedDate;

			productRating.ProductId = ProductId;
			productRating.Rating = Rating;

			return productRating;
		}

		public virtual ProductRatingEntity FromModel(ProductRating productRating, PrimaryKeyResolvingMap pkMap)
		{
			if (productRating == null)
				throw new ArgumentNullException(nameof(productRating));

			pkMap.AddPair(productRating, this);

			Id = productRating.Id;
			CreatedBy = productRating.CreatedBy;
			CreatedDate = productRating.CreatedDate;
			ModifiedBy = productRating.ModifiedBy;
			ModifiedDate = productRating.ModifiedDate;

			ProductId = productRating.ProductId;
			Rating = productRating.Rating;

			return this;
		}

		public virtual void Patch(ProductRatingEntity target)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			target.ProductId = ProductId;
			target.Rating = Rating;
		}
	}
}
