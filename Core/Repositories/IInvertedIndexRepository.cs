using RubberSearch.Core.Models;
namespace RubberSearch.Core.Repositories
{
    public interface IInvertedIndexRepository
    {
        /// <summary>
        /// Gets the path to the index file for the specified tenant.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        string GetIndexFile(string tenantId);
        Task SaveIndexEntriesAsync(IEnumerable<IndexEntry> entries, string tenantId);
        Task<Dictionary<string, IndexEntry>> GetIndexEntriesAsync(string tenantId);
    }
}
