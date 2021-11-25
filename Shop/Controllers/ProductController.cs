using Microsoft.AspNetCore.Mvc;
using Shop.Core.Dtos;
using Shop.Core.ServiceInterface;
using Shop.Data;
using Shop.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly IProductService _productService;

        public ProductController
            (
            ShopDbContext context,
            IProductService productService
            )
        {
            _context = context;
            _productService = productService;
        }

        //ListItem
        [HttpGet]
        public IActionResult Index()
        {
            var result = _context.Product
                .OrderByDescending(y => y.CreatedAt)
                .Select(x => new ProductListItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Value = x.Value,
                    Weight = x.Weight
                });

            return View(result);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ProductViewModel model = new ProductViewModel();

            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductViewModel model)
        {
            var dto = new ProductDto()
            {
                Id = model.Id,
                Description = model.Description,
                Name = model.Name,
                Value = model.Value,
                Weight = model.Weight,
                CreatedAt = model.CreatedAt,
                ModifiedAt = model.ModifiedAt
            };

            var result = await _productService.Add(dto);

            if(result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _productService.Delete(id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _productService.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel();

            model.Id = product.Id;
            model.Name = product.Name;
            model.Description = product.Description;
            model.Value = product.Value;
            model.Weight = product.Weight;
            model.ModifiedAt = product.ModifiedAt;
            model.CreatedAt = product.CreatedAt;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            var dto = new ProductDto()
            {
                Id = model.Id,
                Description = model.Description,
                Name = model.Name,
                Value = model.Value,
                Weight = model.Weight,
                CreatedAt = model.CreatedAt,
                ModifiedAt = model.ModifiedAt
            };

            var result = await _productService.Update(dto);

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), model);
        }
    }
}
