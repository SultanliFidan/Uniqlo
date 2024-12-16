﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Uniqlol.DataAccess;
using Uniqlol.Extensions;
using Uniqlol.Helpers;
using Uniqlol.Models;
using Uniqlol.ViewModels.Commons;
using Uniqlol.ViewModels.Products;

namespace Uniqlol.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleConstants.Product)]
public class ProductController(IWebHostEnvironment _env, UniqloDbContext _context) : Controller
{
    public async Task<IActionResult> Index(int? page = 1,int? take = 1)
    {
        if (!page.HasValue) page = 1;
        if (!take.HasValue) take = 1;
        var query = _context.Products.Include(x => x.Brand).AsQueryable();
        var data = await query.Skip(take.Value * (page.Value - 1)).Take(take.Value).ToListAsync();
        ViewBag.PaginationItems = new PaginationItemsVM(await query.CountAsync(), take.Value, page.Value);
        return View(data);
    }
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {
        if (vm.File != null)
        {
            if (!vm.File.IsValidType("image"))
                ModelState.AddModelError("File", "File must be an image");
            if (!vm.File.IsValidSize(400))
                ModelState.AddModelError("File", "File must be less than 400kb");
        }
        if (vm.OtherFiles != null && vm.OtherFiles.Any())
        {
            if (!vm.OtherFiles.All(x => x.IsValidType("image")))
            {
                string fileNames = string.Join(',', vm.OtherFiles.Where(x => !x.IsValidType("image")).Select(x => x.FileName));
                ModelState.AddModelError("OtherFiles", fileNames + " is (are) not an image");
            }
            if (!vm.OtherFiles.All(x => x.IsValidSize(400)))
            {
                string fileNames = string.Join(',', vm.OtherFiles.Where(x => !x.IsValidSize(400)).Select(x => x.FileName));
                ModelState.AddModelError("OtherFiles", fileNames + " is (are) bigger than 400kb");
            }
        }
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
            return View(vm);
        }
        if (!await _context.Brands.AnyAsync(x => x.Id == vm.BrandId))
        {
            ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
            ModelState.AddModelError("BrandId", "Brand not found");
            return View();
        }
        Product product = vm;
        product.CoverImage = await vm.File!.UploadAsync(_env.WebRootPath, "imgs", "products");
        product.Images = vm.OtherFiles?.Select(x => new ProductImage
        {
            ImageUrl = x.UploadAsync(_env.WebRootPath, "imgs", "products").Result
        }).ToList();
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Update(int? id)
    {
        if (id is null) return BadRequest();
        var data = await _context.Products
            .Where(x => x.Id == id)
            .Select(x => new ProductUpdateVM
            {
                Id = x.Id,
                BrandId = x.BrandId ?? 0,
                CostPrice = x.CostPrice,
                Description = x.Description,
                Discount = x.Discount,
                FileUrl = x.CoverImage,
                Name = x.Name,
                Quantity = x.Quantity,
                SalePrice = x.SalePrice,
                OtherFilesUrls = x.Images.Select(y => y.ImageUrl)
            })
            .FirstOrDefaultAsync();
        if (data is null) return NotFound();
        ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted)
            .ToListAsync();
        return View(data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int? id, ProductUpdateVM vm)
    {
        Product? product = await _context.Products.Include(x => x.Images).Where(x => x.Id == id).FirstOrDefaultAsync();
        if(product is null) return NotFound();

        if(vm.OtherFiles is not null)
        {
				product.Images.AddRange(vm.OtherFiles.Select(x => new ProductImage
				{
					ImageUrl = x.UploadAsync(_env.WebRootPath, "imgs", "products").Result
				}).ToList());
			}
        product.Name = vm.Name;
        product.Description = vm.Description;
        product.CostPrice = vm.CostPrice;
        product.SalePrice = vm.SalePrice;
        product.Discount = vm.Discount;
        product.Quantity = vm.Quantity;
       
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> DeleteImgs(int id, IEnumerable<string> imgNames)
    {
        int result = await _context.ProductImages.Where(x => imgNames.Contains(x.ImageUrl)).ExecuteDeleteAsync();
        if (result > 0)
        {
            // kohne shekilleri sil
        }
        return RedirectToAction(nameof(Update), new { id });
    }


    public async Task<IActionResult> Delete(int id)
    {
        Product product = await _context.Products
             .Include(p => p.ProductComments).Include(p => p.ProductRatings)
             .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        string filePath = Path.Combine(_env.WebRootPath, "imgs", "products", product.CoverImage);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        _context.Products.Remove(product);
        _context.ProductRatings.RemoveRange(product.ProductRatings);
        _context.ProductComments.RemoveRange(product.ProductComments);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}