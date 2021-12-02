using Shop.Core.Domain;
using Shop.Core.Dtos;
using Shop.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Core.ServiceInterface;
using System.IO;
using Microsoft.Extensions.Hosting;


namespace Shop.ApplicationServices.Services
{
    public class ProductServices : IProductService
    {
        private readonly ShopDbContext _context;
        private readonly IHostEnvironment _env;

        public ProductServices
            (
            ShopDbContext context,
            IHostEnvironment env
            )
        {
            _context = context;
            _env = env;
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
            ProcessUploadFile(dto, product);

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
            ExistingFilePath file = new ExistingFilePath();

            product.Id = dto.Id;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Value = dto.Value;
            product.Weight = dto.Weight;
            product.CreatedAt = dto.CreatedAt;
            product.ModifiedAt = DateTime.Now;

            if (dto.Files != null)
            {
                file.FilePath = ProcessUploadFile(dto, product);
            }

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

        public string ProcessUploadFile(ProductDto dto, Product product)
        {
            string uniqueFileName = null;

            if(dto.Files != null && dto.Files.Count > 0)
            {
                if(!Directory.Exists(_env.ContentRootPath + "\\multipleFileUpload\\"))
                {
                    Directory.CreateDirectory(_env.ContentRootPath + "\\multipleFileUpload\\");
                }


                foreach (var photo in dto.Files)
                {
                    string uploadsFolder = Path.Combine(_env.ContentRootPath, "multipleFileUpload");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);

                        ExistingFilePath path = new ExistingFilePath
                        {
                            Id = Guid.NewGuid(),
                            FilePath = uniqueFileName,
                            ProductId = product.Id
                        };

                        _context.ExistingFilePath.Add(path);
                    }
                }
            }

            return uniqueFileName;
        }
    }
}
