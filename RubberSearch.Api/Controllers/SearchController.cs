using Microsoft.AspNetCore.Mvc;
using RubberSearch.Core.Services;
using RubberSearch.Core.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace RubberSearch.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchservice;
        // private readonly ApiKeyMapper _apiKeyMapper;

        public SearchController(ISearchService searchService) // , ApiKeyMapper apiKeyMapper
        {
            _searchservice = searchService;
            //_apiKeyMapper = apiKeyMapper;
        }

        /// <summary>
        /// Search via query string. Authorization: Bearer &lt;apiKey&gt; required.
        /// Example: GET /api/search?q=graph
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string q, int max = 10)
        {
            //var tenantId = ResolveTenantFromHeader();
            var tenantId = User.FindFirst("TenantId")?.Value;
            if (tenantId == null) return Unauthorized("Missing or invalid API key");

            if (string.IsNullOrWhiteSpace(q)) return BadRequest("Query parameter 'q' is required.");

            var coreResults = await _searchservice.SearchAsync(q, tenantId, max);
            var apiResults = coreResults.Select(MapToApi).ToList();
            return Ok(apiResults);
        }

        /// <summary>
        /// Search via JSON body: { "q": "your query", "max": 10 }
        /// Header Authorization: Bearer &lt;apiKey&gt; required.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] QueryRequest? request)
        {
            // var tenantId = ResolveTenantFromHeader();
            var tenantId = User.FindFirst("TenantId")?.Value;
            if (tenantId == null) return Unauthorized("Missing or invalid API key");

            if (request == null || string.IsNullOrWhiteSpace(request.Q)) return BadRequest("Request body must contain 'q'.");

            var coreResults = await _searchservice.SearchAsync(request.Q, tenantId, request.Max ?? 10);
            var apiResults = coreResults.Select(MapToApi).ToList();
            return Ok(apiResults);
        }

        //private string? ResolveTenantFromHeader()
        //{
          //  var header = Request.Headers["Authorization"].FirstOrDefault();
            //var key = header?.Split(' ').LastOrDefault();
            //if (string.IsNullOrWhiteSpace(key)) return null;
            //var tenantId = _apiKeyMapper.GetTenantIdForApiKey(key);
            //return string.IsNullOrWhiteSpace(tenantId) ? null : tenantId;
        //} 

        private static Models.SearchResult MapToApi(Core.Models.SearchResult r) =>
            new Models.SearchResult
            {
                DocId = r.DocId,
                Title = r.Title,
                Snippet = r.Snippet,
                Score = r.Score,
                Url = r.Url
            };

        public class QueryRequest
        {
            public string Q { get; set; } = string.Empty;
            public int? Max { get; set; }
        }
    }
}