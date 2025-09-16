using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using RubberSearch.Core.Services;
using RubberSearch.Core.Models;
using RubberSearch.Core.Repositories;

namespace UnitTests
{
    internal class InMemoryDocumentRepository : IDocumentRepository
    {
        public Dictionary<string, Document> Store { get; } = new();
        public Task<Document> GetDocumentAsync(string docId) => Task.FromResult(Store.ContainsKey(docId) ? Store[docId] : null);
        public Task SaveDocumentAsync(Document document) { Store[document.DocId] = document; return Task.CompletedTask; }
    }

    internal class InMemoryIndexRepository : IInvertedIndexRepository
    {
        public Dictionary<string, IndexEntry> Index { get; } = new();
        public Task<Dictionary<string, IndexEntry>> GetIndexEntriesAsync() => Task.FromResult(new Dictionary<string, IndexEntry>(Index));
        public Task SaveIndexEntriesAsync(IEnumerable<IndexEntry> entries)
        {
            Index.Clear();
            foreach (var e in entries) Index[e.Token] = e;
            return Task.CompletedTask;
        }
    }

    public class IndexingServiceTests
    {
        [Fact]
        public async Task AddDocumentAsync_PersistsDocumentAndIndex()
        {
            var docRepo = new InMemoryDocumentRepository();
            var idxRepo = new InMemoryIndexRepository();
            var service = new IndexingService(docRepo, idxRepo);

            var doc = new Document { DocId = "d1", Title = "Graphs", Content = "Graphs are graphs" };
            await service.AddDocumentAsync(doc);

            Assert.True(docRepo.Store.ContainsKey("d1"));
            Assert.True(idxRepo.Index.Count > 0);
            Assert.Contains(idxRepo.Index, kvp => kvp.Value.Postings.ContainsKey("d1"));
        }
    }
}
