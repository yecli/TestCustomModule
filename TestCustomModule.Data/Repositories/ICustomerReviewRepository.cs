using System.Linq;
using TestCustomModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace TestCustomModule.Data.Repositories
{
    public interface ICustomerReviewRepository : IRepository
    {
        IQueryable<CustomerReviewEntity> CustomerReviews { get; }

        CustomerReviewEntity[] GetByIds(string[] ids);
        void DeleteCustomerReviews(string[] ids);
    }
}
