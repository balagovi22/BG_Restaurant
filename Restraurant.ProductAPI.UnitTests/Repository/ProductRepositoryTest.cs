using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.ProductAPI.Database;
using Restaurant.ProductAPI.Models;
using Restaurant.ProductAPI.Models.Dto;
using Restaurant.ProductAPI.Repository;
using System.Collections;

namespace Restraurant.ProductAPI.UnitTests.Repository
{
    public class ProductRepositoryTest
    {
        private readonly IMapper _mapper;
        public ProductRepositoryTest()
        {
            _mapper = A.Fake<IMapper>();
        }

        private async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.products.CountAsync() <=0 ) {
                databaseContext.products.Add(new Product { Id = 1, Name = "The First 60 days", CategoryName = "Books", Price = 25 });
                databaseContext.products.Add(new Product { Id = 2, Name = "The 5 AM Club", CategoryName = "Books", Price = 20 });
            }
            await databaseContext.SaveChangesAsync();
            return databaseContext;
        }

        //private async Task<IEnumerable<Product>> getProducts()
        //{
        //    var products = new List<Product>()
        //    {
        //        new Product { Id = 1, Name = "The First 60 days", CategoryName = "Books", Price = 25 },
        //        new Product { Id = 2, Name = "The 5 AM Club", CategoryName = "Books", Price = 20 }
        //    };
        //    if (products.Count > 0 )
        //    {
        //        return products;
        //    }
        //    return products;
           
        //}

        //[Fact]
        //public async void Should_return_all_products()
        //{
        //    //Arrange
        //    var dbContext = await GetDatabaseContext();
        //    var productRepository = new ProductRepository(dbContext, _mapper);

        //    //Act
        //    var result = productRepository.GetProducts();

        //    //Assert
        //    result.Should().NotBeNull();
        //    result.Should().BeOfType<Task<IEnumerable<Product>>>();
        //}

        [Fact]
        public async void Should_return_all_products_repo()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var products = A.Fake<IEnumerable<Product>>();
            var productDtos = A.Fake<IEnumerable<ProductDto>>();
            
            A.CallTo(() => _mapper.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);
            var productRepository = new ProductRepository(dbContext, _mapper);

            //Act
            var result = productRepository.GetAllProducts();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IEnumerable<ProductDto>>>();
        }

        [Fact]
        public async void Should_return_all_productdtos_repo()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var products = dbContext.products;
            var productDtos = GetProductDtosList(); //A.Fake<IEnumerable<ProductDto>>();
            A.CallTo(() => _mapper.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);
            var productRepository = new ProductRepository(dbContext, _mapper);

            //Act
            var result = productRepository.GetAllProducts().Result;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<IEnumerable<ProductDto>>();
        }

        [Fact]
        public async void Should_return_all_productdtos_repo_noproducts()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var products =  A.Fake<IEnumerable<Product>>();
            var productDtos = A.Fake<IEnumerable<ProductDto>>();
            A.CallTo(() => _mapper.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);
            var productRepository = new ProductRepository(dbContext, _mapper);

            //Act
            var result = await productRepository.GetAllProducts();
            var obj = result as ObjectResult;

            //Assert
            Assert.Equal(200, obj.StatusCode);
            //result.Should().NotBeNull();
                
            //result.Should().BeOfType<Task<IEnumerable<ProductDto>>>();
        }

        
        private List<ProductDto> GetProductDtosList()
        {

            var products = new List<ProductDto>()
            {
                new ProductDto(){ Id=1, Name="Financial Freedom", CategoryName = "Books", Price=20.00},
                new ProductDto() { Id = 2, Name = "First 90 Days", CategoryName = "Books", Price = 26.00 }
            };

            return products;
        }
    }
}