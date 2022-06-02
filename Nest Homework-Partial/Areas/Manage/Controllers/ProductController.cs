using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest_Homework_Partial.Data_Access_Layer;
using Nest_Homework_Partial.Models;
using Nest_Homework_Partial.Utilies;
using Nest_Homework_Partial.Utilies.Extensions;
using Nest_Homework_Partial.ViewModels.Products;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Nest_Homework_Partial.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private AppDbContext _context { get; }
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> prdcts = await _context.Products.Include(p => p.ProductImages).Include(p => p.Category).ToListAsync();
            List<ProductVM> productVMs = new List<ProductVM>();
            foreach (var item in prdcts)
            {
                ProductVM product = new ProductVM
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category = item.Category.Name,
                    Price = item.Price,
                    Image = item.ProductImages.FirstOrDefault(pi => pi.IsFront == true).Image,
                    IsDeleted = item.IsDeleted
                };
                productVMs.Add(product);
            }
            return View(productVMs);
        }
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (_context.Products.Any(p => p.Name.Trim().ToLower() == product.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", "Bu ad artıq mövcuddur");
                return View();
            }
            if (product.Photos != null)
            {
                foreach (var file in product.Photos)
                {
                    if (IsPhotoOk(file) != "")
                    {
                        ModelState.AddModelError("Photos", IsPhotoOk(file));
                    }
                }
                foreach (var file in product.Photos)
                {
                    ProductImage image = new ProductImage
                    {
                        Image = await file.SaveFileAsync(Path.Combine(Constant.ImagePath, "shop")),
                        IsFront = false,
                        IsBack = false,
                        Product = product
                    };
                    product.ProductImages.Add(image);
                }
            }
            if (IsPhotoOk(product.PhotoFront) != "")
            {
                ModelState.AddModelError("PhotoFront", IsPhotoOk(product.PhotoFront));
            }
            if (IsPhotoOk(product.PhotoBack) != "")
            {
                ModelState.AddModelError("PhotoBack", IsPhotoOk(product.PhotoBack));
            }
            product.ProductImages = new List<ProductImage>();
            product.ProductImages.Add(new ProductImage
            {
                Image = await product.PhotoFront.SaveFileAsync(Path.Combine(Constant.ImagePath, "shop")),
                IsFront = true,
                IsBack = false,
                Product = product
            });
            product.ProductImages.Add(new ProductImage
            {
                Image = await product.PhotoBack.SaveFileAsync(Path.Combine(Constant.ImagePath, "shop")),
                IsFront = false,
                IsBack = true,
                Product = product
            });


            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Product product = _context.Products.Find(id);
            if (product == null) return NotFound();
            if (product.IsDeleted == true)
            {
                _context.Products.Remove(product);
            }
            else
            {
                product.IsDeleted = true;
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int id)
        {
            ViewBag.Categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            Product product = _context.Products.Include(p => p.Category).Include(p => p.ProductImages).SingleOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, Product product)
        {
            return Json(product.PhotoIds);
        }
        private string IsPhotoOk(IFormFile file)
        {
            if (file.CheckSize(500))
            {
                return $"{file.FileName} 500kb-dən az olmalıdır";
            }
            if (!file.CheckType("image/"))
            {
                return $"{file.FileName} şəkil deyil";
            }
            return "";
        }
    }
}
