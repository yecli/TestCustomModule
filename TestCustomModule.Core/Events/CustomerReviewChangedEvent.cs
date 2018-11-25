using System.Collections.Generic;
using TestCustomModule.Core.Model;
using VirtoCommerce.Domain.Common.Events;

namespace TestCustomModule.Core.Events
{
	public class CustomerReviewChangedEvent : GenericChangedEntryEvent<CustomerReview>
	{
		public CustomerReviewChangedEvent(IEnumerable<GenericChangedEntry<CustomerReview>> changedEntries) : base(changedEntries) { }
	}
}
