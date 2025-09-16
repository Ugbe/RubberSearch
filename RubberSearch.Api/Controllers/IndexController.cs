using Microsoft.AspNetCore.Mvc;
using RubberSearch.Api.Models;
using RubberSearch.Core.Services;
using RubberSearch.Core.Models;
using RubberSearch.Core.Utilities;

namespace RubberSearch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndexController : ControllerBase
    {
        private readonly IIndexingService _indexingservice;
        private readonly ApiKeyMapper _apiKeyMapper;
        public IndexController(IIndexingService indexingService, ApiKeyMapper apiKeyMapper)
        {
            _indexingservice = indexingService;
            _apiKeyMapper = apiKeyMapper;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DocumentDto doc)
        {
            var header = Request.Headers["Authorization"].FirstOrDefault();
            var key = header?.Split(' ').LastOrDefault();
            if (string.IsNullOrWhiteSpace(key)) return Unauthorized("Missing API Key in Authoriation header");

            var tenantId = _apiKeyMapper.GetTenantIdForApiKey(key);
            if (string.IsNullOrWhiteSpace(tenantId)) return Unauthorized("Invalid API Key");

            if (doc == null || string.IsNullOrWhiteSpace(doc.DocId) || string.IsNullOrWhiteSpace(doc.Content))
            {
                return BadRequest("Invalid document data.");
            }
    
            var document = new Document
            {
                TenantId = tenantId,
                DocId = doc.DocId,
                Title = doc.Title,
                Content = doc.Content,
                Url = doc.Url
            };
            await _indexingservice.AddDocumentAsync(document, tenantId);

            return Ok(new { message = "Document indexed successfully", document = doc });
        }
    }
}