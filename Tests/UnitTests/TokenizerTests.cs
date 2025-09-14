using Xunit;
using RubberSearch.Core.Utilities;

namespace UnitTests
{
    public class TokenizerTests
    {
        [Fact]
        public void TokenizeWithPositions_ReturnsTokensAndPositions()
        {
            var tokenizer = new Tokenizer(3, 4);
            var result = tokenizer.TokenizeWithPositions("Graph algorithms are fun");

            // 'graph' should appear at position 0
            Assert.True(result.ContainsKey("graph"));
            Assert.Contains(0, result["graph"]);

            // n-gram 'alg' should exist from 'algorithms'
            Assert.True(result.ContainsKey("alg"));
        }
    }
}
