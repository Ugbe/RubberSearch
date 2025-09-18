using System.Text.RegularExpressions;
using RubberSearch.Core.Models;
using RubberSearch.Core.Repositories;
using RubberSearch.Core.Utilities;

namespace RubberSearch.Core.Services
{
    public class SearchService : ISearchService
    {
        private readonly IInvertedIndexRepository _indexRepo;
        private readonly IDocumentRepository _docRepo;
        private readonly Tokenizer _tokenizer;
        private readonly double _titleBoost = 2.0;
        private readonly double _proximityWeight = 2.0;

        public SearchService(IInvertedIndexRepository indexRepo, IDocumentRepository docRepo)
        {
            _indexRepo = indexRepo;
            _docRepo = docRepo;
            _tokenizer = new Tokenizer();
        }

        public async Task<IEnumerable<SearchResult>> SearchAsync(string query, string tenantId, int max = 10)
        {
            if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<SearchResult>();

            // 1. Tokenize query (use words / n-grams consistent with index)
            var qTokens = _tokenizer.Tokenize(query);

            // 2. Load index and compute N
            var index = await _indexRepo.GetIndexEntriesAsync(tenantId);
            var docIdSet = new HashSet<string>(index.Values.SelectMany(e => e.Postings.Keys));
            var N = Math.Max(1, docIdSet.Count);

            // 3. Accumulate TF-IDF scores
            var scores = new Dictionary<string, double>();
            foreach (var token in qTokens.Distinct())
            {
                if (!index.TryGetValue(token, out var entry)) continue;
                var df = Math.Max(1, entry.DocumentFrequency);
                var idf = Math.Log((N + 1.0) / (df + 1.0)) + 1.0;
                foreach (var kv in entry.Postings)
                {
                    var docId = kv.Key;
                    var posting = kv.Value;
                    var tfWeight = posting.TermFrequency > 0 ? 1.0 + Math.Log(posting.TermFrequency) : 1.0;
                    var boost = posting.IsInTitle ? _titleBoost : 1.0;
                    var termScore = tfWeight * idf * boost;
                    if (!scores.ContainsKey(docId)) scores[docId] = 0;
                    scores[docId] += termScore;
                }
            }

            // 4. Proximity boost (if query has >1 token)
            if (qTokens.Count > 1)
            {
                foreach (var docId in scores.Keys.ToList())
                {
                    // compute minimal span across tokens using positions lists from index entries
                    var spans = new List<int>();
                    var tokenPositionsPerToken = new List<List<int>>();
                    foreach (var token in qTokens.Distinct())
                    {
                        if (!index.TryGetValue(token, out var entry) || !entry.Postings.TryGetValue(docId, out var posting)) { tokenPositionsPerToken = null; break; }
                        tokenPositionsPerToken.Add(posting.Positions);
                    }
                    if (tokenPositionsPerToken == null) continue;
                    // simple greedy compute minimal span (approx)
                    int minSpan = int.MaxValue;
                    // flatten approach: for each position of first token find closest positions for others — O(k*m)
                    foreach (var pos0 in tokenPositionsPerToken[0])
                    {
                        int minPos = pos0, maxPos = pos0;
                        bool ok = true;
                        for (int i=1;i<tokenPositionsPerToken.Count;i++)
                        {
                            var closest = tokenPositionsPerToken[i].OrderBy(p => Math.Abs(p - pos0)).FirstOrDefault();
                            if (closest==0 && !tokenPositionsPerToken[i].Contains(0) && tokenPositionsPerToken[i].Count==0) { ok=false; break; }
                            minPos = Math.Min(minPos, closest);
                            maxPos = Math.Max(maxPos, closest);
                        }
                        if (!ok) continue;
                        minSpan = Math.Min(minSpan, maxPos - minPos);
                    }
                    if (minSpan < int.MaxValue)
                    {
                        var proximityBoost = 1.0 + (_proximityWeight / (minSpan + 1.0));
                        scores[docId] *= proximityBoost;
                    }
                }
            }

            // 5. Build results
            var ranked = scores.OrderByDescending(kv => kv.Value).Take(max);
            var results = new List<SearchResult>();
            foreach (var kv in ranked)
            {
                var doc = await _docRepo.GetDocumentAsync(kv.Key, tenantId);
                if (doc == null) continue;
                // build a simple snippet: find first occurrence of any token in content
                var snippet = BuildSnippet(doc.Content, qTokens);
                results.Add(new SearchResult { DocId = doc.DocId, Title = doc.Title, Snippet = snippet, Score = kv.Value, Url = doc.Url });
            }
            return results;
        }

        private string BuildSnippet(string content, List<string> tokens, int window = 40)
        {
            var lower = content.ToLowerInvariant();
            foreach (var t in tokens)
            {
                var idx = lower.IndexOf(t);
                if (idx >= 0)
                {
                    var start = Math.Max(0, idx - window);
                    var len = Math.Min(content.Length - start, window * 2);
                    return (start > 0 ? "..." : "") + content.Substring(start, len) + (start + len < content.Length ? "..." : "");
                }
            }
            return content.Length <= 160 ? content : content.Substring(0, 160) + "...";
        }
    }
}