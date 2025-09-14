namespace RubberSearch.Core.Utilities
{
    public class Tokenizer
    {
        private readonly int _minNGramLength;
        private readonly int _maxNGramLength;

        public Tokenizer(int minNGramLength = 3, int maxNGramLength = 6)
        {
            _minNGramLength = minNGramLength;
            _maxNGramLength = maxNGramLength;
        }

    /// <summary>
    /// Tokenizes text into tokens (n-grams) and returns a map of token -> list of token positions (word offsets).
    /// Positions are word indices (0-based) in the tokenized text.
    /// </summary>
    /// <param name="text">Input text to tokenize.</param>
    /// <returns>Dictionary mapping token to positions where it appears.</returns>
    public Dictionary<string, List<int>> TokenizeWithPositions(string text)
        {
            var tokenPositions = new Dictionary<string, List<int>>();
            if (string.IsNullOrWhiteSpace(text))
                return tokenPositions;

            // Normalize text
            text = text.ToLowerInvariant()
                      .Replace(Environment.NewLine, " ")
                      .Replace("\t", " ");

            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            for (int position = 0; position < words.Length; position++)
            {
                var word = words[position];

                // Add the whole word if it meets length requirements
                if (word.Length >= _minNGramLength)
                {
                    AddTokenPosition(tokenPositions, word, position);
                }

                // Generate n-grams
                for (int n = _minNGramLength; n <= _maxNGramLength && n <= word.Length; n++)
                {
                    for (int i = 0; i <= word.Length - n; i++)
                    {
                        var ngram = word.Substring(i, n);
                        AddTokenPosition(tokenPositions, ngram, position);
                    }
                }
            }

            return tokenPositions;
        }

    /// <summary>
    /// Helper to add a position for a token into the tokenPositions map.
    /// </summary>
    private void AddTokenPosition(Dictionary<string, List<int>> tokenPositions, string token, int position)
        {
            if (!tokenPositions.TryGetValue(token, out var positions))
            {
                positions = new List<int>();
                tokenPositions[token] = positions;
            }
            positions.Add(position);
        }
    }
}