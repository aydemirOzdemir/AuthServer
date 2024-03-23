using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : CustomBaseController
    {
        private readonly Core.Services.IGenericService<Product,ProductDto> productService;

        public ProductController(IGenericService<Product, ProductDto> productService)
        {
            this.productService = productService;
        }


        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            return ActionResultInstance(await productService.GetAllAsync());
        }
        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductDto productDto)
        {
            return ActionResultInstance(await productService.Add(productDto));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductDto productDto)
        {
            return ActionResultInstance(await  productService.Update(productDto,productDto.Id));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return ActionResultInstance(await productService.Remove(id));
        }
    }
}
