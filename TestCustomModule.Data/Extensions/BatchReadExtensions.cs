using System;
using System.Collections.Generic;
using VirtoCommerce.Domain.Commerce.Model.Search;

namespace TestCustomModule.Data.Extensions
{
	public static class BatchReadExtensions
	{
		private static readonly int MaxBatchSize = 10000;

		/// <summary>
		/// Read entities by batches using search criteria (note that criteria Skip and Take fields could be modified)
		/// </summary>
		/// <typeparam name="TEntity">Type of entites to be read</typeparam>
		/// <typeparam name="TCriteria">Criteria type that inherits from SearchCriteriaBase</typeparam>
		/// <param name="criteria">Search criteria</param>
		/// <param name="getter">The function that return entities collection based on search criteria</param>
		/// <param name="batchSize">Count of entities in one batch</param>
		/// <returns>Returns enumerable with collections </returns>
		public static IEnumerable<ICollection<TEntity>> ReadAllEntitiesByBatches<TCriteria, TEntity>(this TCriteria criteria, Func<TCriteria, ICollection<TEntity>> getter, int batchSize = 5) where TCriteria : SearchCriteriaBase
		{
			ValidateParameters(batchSize);

			ICollection<TEntity> batchResult = null;
			int iteration = 0;
			do
			{
				criteria.Skip = iteration * batchSize;
				criteria.Take = batchSize;
				batchResult = getter(criteria);
				if (batchResult.Count > 0)
				{
					yield return batchResult;
				}
				iteration++;
			}
			while (batchResult.Count == batchSize);
		}

		private static void ValidateParameters(int batchSize)
		{
			if (batchSize <= 0 && batchSize > MaxBatchSize)
			{
				throw new ArgumentException($"{nameof(batchSize)} should be positive and not greater than {MaxBatchSize}");
			}
		}
	}
}
