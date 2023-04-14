using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.ProductAPI.Database;
using Restaurant.ProductAPI.Models;
using Restaurant.ProductAPI.Models.Dto;
using System.Collections.Generic;

namespace Restaurant.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ProductDto> AddModifyProduct(ProductDto productDto)
        {
            Product product = _mapper.Map<Product>(productDto);
            if (product.Id > 0)
            {
                _context.products.Update(product);
            }
            else
            {
                _context.products.Add(product);
            }
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);   
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                var product = await _context.products.FirstOrDefaultAsync(u=>u.Id==productId);
                if (product == null) { return false; }
                _context.products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ProductDto> GetProductById(int id)
        {
            var product = await _context.products.Where(x=>x.Id== id).FirstOrDefaultAsync();
            return _mapper.Map<ProductDto>(product);
        }


        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            List<Product> products = await _context.products.ToListAsync();
            try
            {
                var response = _mapper.Map<List<ProductDto>>(products);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<ProductDto>();
            }
        }
    }
}
