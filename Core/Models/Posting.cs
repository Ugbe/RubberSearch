namespace RubberSearch.Core.Models
{
    public class Posting
    {
    /// <summary>
    /// Document identifier where the token appears.
    /// </summary>
        public string DocId { get; set; } = string.Empty;
    /// <summary>
    /// Term positions (token offsets) within the document - useful for phrase/proximity queries.
    /// </summary>
        public List<int> Positions { get; set; } = new();
    /// <summary>
    /// Number of times the token appears in the document.
    /// </summary>
        public int TermFrequency { get; set; }
    /// <summary>
    /// Whether the token appears in the document title (useful for boosting).
    /// </summary>
        public bool IsInTitle { get; set; }
    }
}