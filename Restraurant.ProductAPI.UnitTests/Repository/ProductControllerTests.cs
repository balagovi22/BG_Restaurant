using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Restaurant.ProductAPI.Controllers;
using Restaurant.ProductAPI.Models;
using Restaurant.ProductAPI.Models.Dto;
using Restaurant.ProductAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.ProductAPI.Tests.Repository
{
    
    public class ProductControllerTestsShould
    {
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IMapper> _mapper;
        private Fixture _fixture;
        private ProductAPIController _productApiController;

        public ProductControllerTestsShould()
        {
            _fixture= new Fixture();
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        [Trait("Category", "GetAllProductsTests")]
        public async Task Validate_AllProducts_AreOfTypeProductDto()
        {
            var productList = _fixture.CreateMany<ProductDto>(3).ToList();
            _productRepositoryMock.Setup(x =>  x.GetAllProducts().Result).Returns(productList);
            _productApiController = new ProductAPIController(_productRepositoryMock.Object, _mapper.Object);
            var result = await _productApiController.GetAllProducts();
            result.Should().BeOfType<ProductResponseDto>();
        }
        [Fact]
        [Trait("Category", "GetAllProductsTests")]
        public async Task Validate_IsSuccessFlagIsFalse_WhenExceptionisThrown()
        {
            var productList = _fixture.CreateMany<ProductDto>(3).ToList();
            _productRepositoryMock.Setup(x => x.GetAllProducts().Result).Throws<Exception>();
            _productApiController = new ProductAPIController(_productRepositoryMock.Object, _mapper.Object);
            var result = await _productApiController.GetAllProducts();
            Assert.False(result.IsSuccess);
        }
        [Fact]
        [Trait("Category", "GetAllProductsTests")]
        public async Task Validate_ResponseType_WhenExceptionisThrown()
        {
            var productList = _fixture.CreateMany<ProductDto>(3).ToList();
            _productRepositoryMock.Setup(x => x.GetAllProducts().Result).Throws<Exception>();
            _productApiController = new ProductAPIController(_productRepositoryMock.Object, _mapper.Object);
            var result = await _productApiController.GetAllProducts();
            result.Should().BeOfType<ProductResponseDto>();
        }
        [Fact]
        [Trait("Category", "GetAllProductsTests")]
        public async Task Validate_ErrorMessageCollectionGreaterThanZero_WhenExceptionisThrown()
        {
            var productList = _fixture.CreateMany<ProductDto>(3).ToList();
            _productRepositoryMock.Setup(x => x.GetAllProducts().Result).Throws<Exception>();
            _productApiController = new ProductAPIController(_productRepositoryMock.Object, _mapper.Object);
            var result = await _productApiController.GetAllProducts();
            Assert.True(result.ErrorMessages.Count() > 0);
        }
        [Fact]
        [Trait("Category", "GetAllProductsByIdTests")]
        public async Task Validate_ResponseType_WhenGettingProductById()
        {
            ProductDto productDto = _fixture.CreateMany<ProductDto>(1).Single();
            _productRepositoryMock.Setup(x => x.GetProductById(It.IsAny<int>()).Result).Returns(productDto);
            _productApiController = new ProductAPIController(_productRepositoryMock.Object, _mapper.Object);
            var productId = productDto.Id;
            var result = await _productApiController.GetProductById(productId);
            result.Should().BeOfType<ProductResponseDto>();
            Assert.True(result.IsSuccess);
            var productIdFromRepository = ((ProductDto)result.Result).Id;
            Assert.Equal(productDto.Id, productIdFromRepository);
        }
        [Fact]
        [Trait("Category", "GetAllProductsByIdTests")]
        public async Task Validate_IsSuccessFlag_WhenGettingProductById()
        {
            ProductDto productDto = _fixture.CreateMany<ProductDto>(1).Single();
            _productRepositoryMock.Setup(x => x.GetProductById(It.IsAny<int>()).Result).Returns(productDto);
            _productApiController = new ProductAPIController(_productRepositoryMock.Object, _mapper.Object);
            var productId = productDto.Id;
            var result = await _productApiController.GetProductById(productId);
            Assert.True(result.IsSuccess);
        }
        [Fact]
        [Trait("Category", "GetAllProductsByIdTests")]
        public async Task Validate_ProductIdFromRepoMatchesProductIdPassedIn_WhenGettingProductById()
        {
            ProductDto productDto = _fixture.CreateMany<ProductDto>(1).Single();
            _productRepositoryMock.Setup(x => x.GetProductById(It.IsAny<int>()).Result).Returns(productDto);
            _productApiController = new ProductAPIController(_productRepositoryMock.Object, _mapper.Object);
            var productId = productDto.Id;
            var result = await _productApiController.GetProductById(productId);
            var productIdFromRepository = ((ProductDto)result.Result).Id;
            Assert.Equal(productDto.Id, productIdFromRepository);
        }
    }
}
