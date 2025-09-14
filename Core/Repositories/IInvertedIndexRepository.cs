using RubberSearch.Core.Models;
namespace RubberSearch.Core.Repositories
{
    public interface IInvertedIndexRepository
    {
        Task SaveIndexEntriesAsync(IEnumerable<IndexEntry> entries);
        Task<Dictionary<string, IndexEntry>> GetIndexEntriesAsync();
    }
}
