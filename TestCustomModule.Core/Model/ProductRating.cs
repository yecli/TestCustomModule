using VirtoCommerce.Platform.Core.Common;

namespace TestCustomModule.Core.Model
{
	public class ProductRating : AuditableEntity
	{
		public string ProductId { get; set; }
		public decimal Rating { get; set; }
	}
}
