using RubberSearch.Core.Models;
using RubberSearch.Core.Utilities;
using RubberSearch.Core.Repositories;

namespace RubberSearch.Core.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IIndexingService"/> that coordinates
    /// tokenization and persistence through repositories.
    /// </summary>
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

    /// <summary>
    /// Persist the document and update the inverted index with postings (positions, frequency).
    /// </summary>
    public async Task AddDocumentAsync(Document document)
        {
            // 1. Save the document
            await _documentRepository.SaveDocumentAsync(document);

            // 2. Extract tokens with positions from title and content
            var titleTokens = _tokenizer.TokenizeWithPositions(document.Title);
            var contentTokens = _tokenizer.TokenizeWithPositions(document.Content);

            // 3. Get existing index
            var indexEntries = await _indexRepository.GetIndexEntriesAsync();

            // 4. Update index entries for this document
            var allTokens = new HashSet<string>(titleTokens.Keys.Union(contentTokens.Keys));
            
            foreach (var token in allTokens)
            {
                if (!indexEntries.TryGetValue(token, out var entry))
                {
                    entry = new IndexEntry { Token = token };
                    indexEntries[token] = entry;
                }

                var posting = new Posting
                {
                    DocId = document.DocId,
                    IsInTitle = titleTokens.ContainsKey(token)
                };

                // Combine positions from title and content
                var positions = new List<int>();
                if (titleTokens.TryGetValue(token, out var titlePositions))
                {
                    positions.AddRange(titlePositions);
                }
                if (contentTokens.TryGetValue(token, out var contentPositions))
                {
                    positions.AddRange(contentPositions);
                }

                posting.Positions = positions;
                posting.TermFrequency = positions.Count;

                entry.Postings[document.DocId] = posting;
            }

            // 5. Save updated index
            await _indexRepository.SaveIndexEntriesAsync(indexEntries.Values);
        }

    /// <summary>
    /// Load a document by id from the document repository.
    /// </summary>
    public async Task<Document?> GetDocumentAsync(string docId)
        {
            return await _documentRepository.GetDocumentAsync(docId);
        }
    }
}