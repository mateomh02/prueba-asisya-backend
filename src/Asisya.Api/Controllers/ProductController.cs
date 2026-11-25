using Asisya.Application.Products.Dto;
using Asisya.Application.Products.Dto;
using Asisya.Application.Products.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asisya.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("Products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductBulkService _bulkService;

        public ProductController(IProductService productService, IProductBulkService bulkService)
        {
            _productService = productService;
            _bulkService = bulkService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductQuery query)
        {
            var result = await _productService.GetAllAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("~/Product")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDetailResponse>> Create(
            UpsertProductRequest request, CancellationToken ct)
        {
            var result = await _productService.CreateAsync(request, ct);
            return Created($"/Products/{result.Id}", result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BulkCreateProductsResponse>> GenerateRandom(
            BulkCreateProductsRequest request, CancellationToken ct)
        {
            var result = await _bulkService.GenerateRandomAsync(request, ct);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDetailResponse>> Update(
            int id, UpsertProductRequest request, CancellationToken ct)
        {
            var result = await _productService.UpdateAsync(id, request, ct);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _productService.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
