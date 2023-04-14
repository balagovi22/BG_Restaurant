using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.ProductAPI.Models.Dto;
using Restaurant.ProductAPI.Repository;

namespace Restaurant.ProductAPI.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ProductResponseDto _responseDto;
        private readonly IMapper _mapper;

        public ProductAPIController(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _responseDto = new ProductResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ProductResponseDto> GetAllProducts() {
            try
            {
                var productDtos = await _repository.GetAllProducts();
                _responseDto.Result = productDtos;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ProductResponseDto> GetProductById(int id)
        {
            try
            {
                var product = await _repository.GetProductById(id);
                _responseDto.Result = product;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }

        [HttpPut]
        [Authorize]
        public async Task<ProductResponseDto> UpdateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var product = await _repository.AddModifyProduct(productDto);
                _responseDto.Result = product;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }

        [HttpPost]
        [Authorize]
        public async Task<ProductResponseDto> AddProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var product = await _repository.AddModifyProduct(productDto);
                _responseDto.Result = product;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }

        [HttpDelete]
        [Authorize(Roles ="Admin")]
        [Route("{id}")]
        public async Task<object> DeleteProduct(int id)
        {
            try
            {
                bool isSuccess = await _repository.DeleteProduct(id);
                _responseDto.Result = isSuccess;
                return true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }
    }
}
