﻿using BlueTapeCrew.Areas.Admin.Models;
using BlueTapeCrew.Data;
using BlueTapeCrew.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace BlueTapeCrew.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminCategoriesController : Controller
    {
        private readonly BtcEntities _db;

        public AdminCategoriesController(BtcEntities db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _db.Categories.FindAsync(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            var existingCategory = _db.Categories.Find(category.Id);
            if (existingCategory != null) existingCategory.CategoryName = category.CategoryName;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            var categories =
            _db.Categories.OrderBy(category => category.CategoryName)
            .Select(category => new AdminCategoryViewModel
            {
                Id = category.Id,
                Name = category.CategoryName,
                ImageId = category.ImageId,
                Published = category.Published,
                Products = category.ProductCategories.Select(x=>x.Product).OrderBy(product => product.ProductName).Select(product => new AdminProductViewModel
                {
                    Description = product.Description,
                    Id = product.Id,
                    ImageId = product.ImageId,
                    Name = product.ProductName
                }).ToList()
            }).ToList();
            ViewBag.ProductId = new SelectList(_db.Products.OrderBy(x => x.ProductName).ToList(), "Id", "ProductName");
            return View(categories);
        }

        public ActionResult Delete(int id)
        {
            var cat = _db.Categories.Find(id);
            if (cat == null) return NotFound();

            var products = cat.ProductCategories.ToList();
            foreach (var product in products) cat.ProductCategories.Remove(product);
            _db.Categories.Remove(cat);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Index(string categoryName)
        {
            var model = new Category { CategoryName = categoryName };
            _db.Categories.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
            return RedirectToAction("Index", "AdminProducts");
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToCategory(int productId, int categoryId)
        {
            _db.ProductCategories.Add(new ProductCategory {ProductId = productId, CategoryId = categoryId});
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProductFromCategory(int productId, int categoryId)
        {
            _db.ProductCategories.Add(new ProductCategory {CategoryId = categoryId, ProductId = productId});
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PublishCategory(int id)
        {
            var category = _db.Categories.Find(id);
            if (category != null) category.Published = !category.Published;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public new void Dispose()
        {
            _db?.Dispose();
        }
    }
}