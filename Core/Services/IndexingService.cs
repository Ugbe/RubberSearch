using RubberSearch.Core.Models;
using RubberSearch.Core.Utilities;
using RubberSearch.Infrastructure.Interfaces;

namespace RubberSearch.Core.Services
{
    public class IndexingService : IIndexingService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IInvertedIndexRepository _indexRepository;
        private readonly Tokenizer _tokenizer;

        public IndexingService(
            IDocumentRepository documentRepository,
            IInvertedIndexRepository indexRepository)
        {
            _documentRepository = documentRepository;
            _indexRepository = indexRepository;
            _tokenizer = new Tokenizer();
        }

        public async Task AddDocumentAsync(Document document)
        {
            // 1. Save the document
            await _documentRepository.SaveDocumentAsync(document);

            // 2. Extract tokens from title and content
            var titleTokens = _tokenizer.Tokenize(document.Title);
            var contentTokens = _tokenizer.Tokenize(document.Content);

            // 3. Get existing index
            var indexEntries = await _indexRepository.GetIndexEntriesAsync();

            // 4. Update index entries for this document
            foreach (var token in titleTokens.Union(contentTokens))
            {
                if (!indexEntries.TryGetValue(token, out var entry))
                {
                    entry = new IndexEntry { Token = token };
                    indexEntries[token] = entry;
                }

                entry.DocumentIds.Add(document.DocId);
            }

            // 5. Save updated index
            await _indexRepository.SaveIndexEntriesAsync(indexEntries.Values);
        }

        public async Task<Document?> GetDocumentAsync(string docId)
        {
            return await _documentRepository.GetDocumentAsync(docId);
        }
    }
}