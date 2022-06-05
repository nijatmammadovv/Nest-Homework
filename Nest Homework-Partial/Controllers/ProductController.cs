using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest_Homework_Partial.Data_Access_Layer;
using Nest_Homework_Partial.Models;
using Nest_Homework_Partial.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_Homework_Partial.Controllers
{
    public class ProductController : Controller
    {
        private AppDbContext _context { get; }
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.ProductCount = _context.Products.Where(p => p.IsDeleted == false).Count();
            ViewBag.Categories = _context.Categories.Where(p => p.IsDeleted == false).Include(c => c.Products);
            return View();
        }
        public IActionResult LoadMore(int skip)
        {
            IQueryable<Product> p = _context.Products.Where(p => p.IsDeleted == false);
            int productCount = p.Count();
            if (productCount <= skip)
            {
                return Json(new
                {
                    message = "Yalnis melumat daxil etmek qadagandir hormetli sexs"
                });
            }
            return PartialView("_ProductPartial", p
                                    .OrderByDescending(p => p.Id)
                                    .Skip(skip)
                                    .Take(10)
                                    .Include(p => p.ProductImages)
                                    .Include(p => p.Category));
        }
        public IActionResult CategoryFilter(int CategoryId)
        {
            if (_context.Categories.Find(CategoryId) == null) return NotFound();
            return PartialView("_ProductPartial", _context.Products.Where(p => p.IsDeleted == false && p.CategoryId == CategoryId)
                                .OrderByDescending(p => p.Id)
                                .Include(p => p.ProductImages)
                                .Include(p => p.Category));
        }
        public IActionResult Basket()
        {
            List<BasketVM> product = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["Basket"]);
            return Json(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) return BadRequest();
            Product dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null) return NotFound();
            UpdateBasket((int)id);
            return RedirectToAction(nameof(Index));
        }

        private List<BasketVM> GetBasket()
        {
            List<BasketVM> basketItems = new List<BasketVM>();
            if (Request.Cookies["Basket"] != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["Basket"]);
            }
            return basketItems;
        }
        private void UpdateBasket(int id)
        {
            List<BasketVM> basketItems = GetBasket();
            BasketVM basketItem = basketItems.Find(bi => bi.ProductId == id);
            if (basketItem != null)
            {
                basketItem.Count += 1;
            }
            else
            {
                basketItem = new BasketVM
                {
                    ProductId = id,
                    Count = 1
                };
                basketItems.Add(basketItem);
            }
            Response.Cookies.Append("Basket", JsonConvert.SerializeObject(basketItems));
        }

    }
}
