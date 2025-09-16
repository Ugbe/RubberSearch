using RubberSearch.Core.Models;

namespace RubberSearch.Core.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<SearchResult>> SearchAsync(string query, string tenantId, int max = 10);
    }
}