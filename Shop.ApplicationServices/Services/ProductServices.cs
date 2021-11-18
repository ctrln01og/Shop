using Shop.Core.Domain;
using Shop.Core.Dtos;
using Shop.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Core.ServiceInterface;

namespace Shop.ApplicationServices.Services
{
    public class ProductServices : IProductService
    {
        private readonly ShopDbContext _context;

        public ProductServices(
            ShopDbContext context
            )
        {
            _context = context;
        }

        public async Task<Product> Add(ProductDto dto)
        {
            Product product = new Product();

            product.Id = Guid.NewGuid();
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Value = dto.Value;
            product.Weight = dto.Weight;
            product.CreatedAt = DateTime.Now;
            product.ModifiedAt = DateTime.Now;

            await _context.Product.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }


        public async Task<Product> Delete(Guid id)
        {
            var productId = await _context.Product
                .FirstOrDefaultAsync(x => x.Id == id);

            _context.Product.Remove(productId);
            await _context.SaveChangesAsync();

            return productId;
        }


        public async Task<Product> Update(ProductDto dto)
        {
            Product product = new Product();

            product.Id = dto.Id;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Value = dto.Value;
            product.Weight = dto.Weight;
            product.CreatedAt = dto.CreatedAt;
            product.ModifiedAt = DateTime.Now;

            _context.Product.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetAsync(Guid id)
        {
            var result = await _context.Product
                .FirstOrDefaultAsync(x => x.Id == id);

            return result;
        }
    }
}
