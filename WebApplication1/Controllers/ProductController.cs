using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurant.Web.Models.Dto;
using Restaurant.Web.Services.IServices;
using System.Reflection;

namespace Restaurant.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }
        public async Task<IActionResult> Index()
        {
            List<ProductDto> products = new();
            var response = await _service.GetAllProductAsync<ResponseDto>();
            if (response != null && response.IsSuccess) {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.CreateProductAsync<ResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int productId)
        {
            var response = await _service.GetProductByIdAsync<ResponseDto>(productId);
            if (response != null && response.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.UpdateProductAsync<ResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var response = await _service.GetProductByIdAsync<ResponseDto>(productId);
            if (response != null && response.IsSuccess)
            {
                ProductDto productToDelete = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(productToDelete);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.DeleteProductAsync<ResponseDto>(model.Id);
                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
