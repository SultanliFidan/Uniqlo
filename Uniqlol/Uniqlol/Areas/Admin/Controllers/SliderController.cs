using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Uniqlol.DataAccess;
using Uniqlol.Extensions;
using Uniqlol.Models;
using Uniqlol.ViewModels.Sliders;

namespace Uniqlol.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SliderController(UniqloDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.ToListAsync());
        }

       
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SliderCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            if (!vm.File.ContentType.StartsWith("image"))
            {
                ModelState.AddModelError("File", "Fayl şəkil formatında olmalıdır");
                return View(vm);
            }
            if (vm.File.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("File", "Fayl 2 Mb-dan az olmalıdır");
                return View(vm);
            }
            string newFileName = Path.GetRandomFileName() + Path.GetExtension(vm.File.FileName);

            using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "sliders", newFileName)))
            {
                await vm.File.CopyToAsync(stream);
            }
            Slider slider = new Slider
            {
                ImageUrl = newFileName,
                Title = vm.Title,
                Subtitle = vm.Subtitle!,
                Link = vm.Link
            };
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider is null) 
            { 
                return NotFound();
            }
            var path = Path.Combine("wwwroot","imgs","sliders",slider.ImageUrl);
            if (System.IO.File.Exists(path))
            { 
                System.IO.File.Delete(path); 
            }
            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hide(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            slider.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Show(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            slider.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Update(int id)
        {
           


            var data = await _context.Sliders.FindAsync(id);
            if (data is null) return NotFound();
            return View(data);


        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, SliderCreateVM vm)
        {

            if (vm.File != null)
            {
                if (!vm.File.IsValidType("image"))
                    ModelState.AddModelError("File", "File must be an image");
                if (!vm.File.IsValidSize(400))
                    ModelState.AddModelError("File", "File must be less than 400kb");
            }
            var entity = await _context.Sliders.FindAsync(id);
            if (entity is null) return NotFound();
            entity.Title = vm.Title;
            entity.Subtitle = vm.Subtitle;
            entity.Link = vm.Link;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}


