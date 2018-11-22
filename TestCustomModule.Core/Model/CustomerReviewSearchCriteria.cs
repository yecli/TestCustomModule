using VirtoCommerce.Domain.Commerce.Model.Search;

namespace TestCustomModule.Core.Model
{
    public class CustomerReviewSearchCriteria : SearchCriteriaBase
    {
        public string[] ProductIds { get; set; }
        public bool? IsActive { get; set; }
    }
}
