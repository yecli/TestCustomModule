using TestCustomModule.Core.Model;
using VirtoCommerce.Domain.Commerce.Model.Search;

namespace TestCustomModule.Core.Services
{
    public interface ICustomerReviewSearchService
    {
        GenericSearchResult<CustomerReview> SearchCustomerReviews(CustomerReviewSearchCriteria criteria);
    }
}
