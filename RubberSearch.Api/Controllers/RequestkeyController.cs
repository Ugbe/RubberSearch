using Microsoft.AspNetCore.Mvc;
using RubberSearch.Core.Utilities;

namespace RubberSearch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestkeyController : ControllerBase
    {
        private readonly ApiKeyMapper _apiKeyMapper;

        public RequestkeyController(ApiKeyMapper apiKeyMapper)
        {
            _apiKeyMapper = apiKeyMapper;
        }

        [HttpGet]
        public IActionResult Requestkey()
        {
            var plainKey = _apiKeyMapper.CreateTenant();
            return Ok(new { apikey = plainKey });
        }
    }
}