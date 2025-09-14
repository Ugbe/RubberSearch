namespace RubberSearch.Core.Models
{
    public class IndexEntry
    {
    /// <summary>
    /// The token/term this index entry represents.
    /// </summary>
        public string Token { get; set; } = string.Empty;
    /// <summary>
    /// Postings map: document id -> posting metadata (positions, frequency, etc.).
    /// </summary>
        public Dictionary<string, Posting> Postings { get; set; } = new();
    /// <summary>
    /// Number of documents that contain this token.
    /// </summary>
        public int DocumentFrequency => Postings.Count;
    }
}