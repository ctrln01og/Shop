using Microsoft.AspNetCore.Mvc;
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

        public ProductController
            (
            ShopDbContext context
            )
        {
            _context = context;
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
    }
}
