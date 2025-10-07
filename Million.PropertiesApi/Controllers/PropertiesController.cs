using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Million.PropertiesApi.Business.Interfaces;
using Million.PropertiesApi.Core.Dtos;

namespace Million.PropertiesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _svc;
        public PropertiesController(IPropertyService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? name, [FromQuery] string? address,
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var filter = new PropertyFilter(name, address, minPrice, maxPrice, page, pageSize);
            var result = await _svc.GetPropertiesAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var prop = await _svc.GetByIdAsync(id);
            if (prop == null) return NotFound();
            return Ok(prop);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromForm] PropertyDetailsImageDto items)
        {
            if (items == null) return BadRequest("Invalid data.");

            await _svc.AddAsync(items);
            return Ok();
        }

    }
}
