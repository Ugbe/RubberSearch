namespace RubberSearch.Core.Utilities
{
    public class Tokenizer
    {
        private readonly int _minNGramLength;
        private readonly int _maxNgramLength;

        public Tokenizer(int minNGramLength = 3, int maxNGramLength = 6)
        {
            _minNGramLength = minNGramLength;
            _maxNgramLength = maxNGramLength;
        }

        public IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return Enumerbale.Empty<string>();

            // Normalize text: lowercase, remove special characters
            text = text.ToLowerInvariant().Replace(Environment.NewLine, " ").Replace("\t", " ");
            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var tokens = new HashSet<string>();

            foreach (var word in words)
            {
                // Add the whole word
                if (word.Length >= _minNGramLength) tokens.Add(word);

                // Generate n-grams
                for (int n = _minNGramLength; n <= _maxNgramLength && n <= word.Length; n++)
                {
                    for (int i = 0; i <= word.Length - n; i++)
                    {
                        tokens.Add(word.Substring(i, n));
                    }
                }
            }
            return tokens;
        }
    }
}