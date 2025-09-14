using Microsoft.AspNetCore.Mvc;
using RubberSearch.Api.Models;
using RubberSearch.Core.Services;
using RubberSearch.Core.Models;

namespace RubberSearch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndexController : ControllerBase
    {
        private readonly IIndexingService _indexingservice;
        public IndexController(IIndexingService indexingService)
        {
            _indexingservice = indexingService;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DocumentDto doc)
        {
            if (doc == null || string.IsNullOrWhiteSpace(doc.DocId))
            {
                return BadRequest("Invalid document data.");
            }
            var document = new Document
            {
                DocId = doc.DocId,
                Title = doc.Title,
                Content = doc.Content,
                Url = doc.Url
            };
            await _indexingservice.AddDocumentAsync(document);

            return Ok(new { message = "Document indexed successfully", document = doc });
        }

        [HttpGet("{docId}")]
        public async Task<IActionResult> Get(string docId)
        {
            var document = await _indexingservice.GetDocumentAsync(docId);
            if (document == null) return NotFound();
            var dto = new DocumentDto
            {
                DocId = document.DocId,
                Title = document.Title,
                Content = document.Content,
                Url = document.Url
            };
            return Ok(dto);
        }
    }
}