namespace RubberSearch.Infrastructure.Interfaces
{
    public interface IInvertedIndexRepository
    {
        Task SaveIndexEntriesAsync(IEnumerable<IndexEntry> entries);
        Task<Dictionary<string, IndexEntry>> GetIndexEntriesAsync();
    }
}